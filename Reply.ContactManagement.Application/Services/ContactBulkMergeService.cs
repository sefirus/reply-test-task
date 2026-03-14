using Microsoft.EntityFrameworkCore;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Core.Contracts.Contacts;
using Reply.ContactManagement.Core.Entities;
using Reply.ContactManagement.Core.Exceptions;
using Reply.ContactManagement.Db;
using System.Text.Json;

namespace Reply.ContactManagement.Application.Services;

public sealed class ContactBulkMergeService(ContactManagementDbContext dbContext) : IContactBulkMergeService
{
    public async Task<BulkMergeContactsResponse> MergeAsync(
        BulkMergeContactsRequest request,
        CancellationToken cancellationToken = default)
    {
        var referencedCustomFieldKeys = request.Contacts
            .SelectMany(contact => contact.CustomFields ?? [])
            .Select(customField => customField.Key.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var customFieldLookup = await dbContext.CustomFields
            .AsNoTracking()
            .Where(customField => referencedCustomFieldKeys.Contains(customField.Name))
            .ToListAsync(cancellationToken);

        var customFieldsByName = customFieldLookup.ToDictionary(
            customField => customField.Name,
            StringComparer.OrdinalIgnoreCase);

        var missingCustomFields = referencedCustomFieldKeys
            .Where(key => !customFieldsByName.ContainsKey(key))
            .ToArray();

        if (missingCustomFields.Length > 0)
        {
            throw new InvalidRequestException($"Unknown custom fields: {string.Join(", ", missingCustomFields)}.");
        }

        var existingContacts = await dbContext.Contacts
            .Include(contact => contact.CustomFieldValues)
            .ThenInclude(value => value.CustomField)
            .ToListAsync(cancellationToken);

        var duplicateExistingEmails = existingContacts
            .Where(contact => !string.IsNullOrWhiteSpace(contact.Email))
            .GroupBy(contact => NormalizeEmail(contact.Email!), StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicateExistingEmails.Length > 0)
        {
            throw new ConflictException($"Duplicate emails already exist in the database: {string.Join(", ", duplicateExistingEmails)}.");
        }

        var existingContactsByEmail = existingContacts
            .Where(contact => !string.IsNullOrWhiteSpace(contact.Email))
            .ToDictionary(contact => NormalizeEmail(contact.Email!), StringComparer.OrdinalIgnoreCase);

        var inputEmails = request.Contacts
            .Select(contact => NormalizeEmail(contact.Email))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var createdCount = 0;
        var updatedCount = 0;

        foreach (var requestContact in request.Contacts)
        {
            var normalizedEmail = NormalizeEmail(requestContact.Email);
            if (!existingContactsByEmail.TryGetValue(normalizedEmail, out var contact))
            {
                contact = new Contact
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = DateTimeOffset.UtcNow
                };

                dbContext.Contacts.Add(contact);
                existingContactsByEmail[normalizedEmail] = contact;
                createdCount++;
            }
            else
            {
                updatedCount++;
            }

            contact.Name = requestContact.Name.Trim();
            contact.Email = NormalizeNullable(requestContact.Email);
            contact.PhoneNumber = NormalizeNullable(requestContact.PhoneNumber);
            contact.Birthday = requestContact.Birthday;
            contact.UpdatedAtUtc = DateTimeOffset.UtcNow;

            var syncResult = SyncCustomFields(contact, requestContact, customFieldsByName);
            if (syncResult is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw syncResult;
            }
        }

        var contactsToDelete = existingContacts
            .Where(contact => contact.Email is null || !inputEmails.Contains(NormalizeEmail(contact.Email)))
            .ToArray();

        if (contactsToDelete.Length > 0)
        {
            dbContext.Contacts.RemoveRange(contactsToDelete);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new BulkMergeContactsResponse(createdCount, updatedCount, contactsToDelete.Length);
    }

    private Exception? SyncCustomFields(
        Contact contact,
        BulkMergeContactRequest requestContact,
        IReadOnlyDictionary<string, CustomField> customFieldsByName)
    {
        var requestedCustomFields = requestContact.CustomFields ?? [];
        var requestedKeys = requestedCustomFields
            .Select(customField => customField.Key.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var existingValue in contact.CustomFieldValues.ToArray())
        {
            if (!requestedKeys.Contains(existingValue.CustomField.Name))
            {
                dbContext.ContactCustomFieldValues.Remove(existingValue);
                contact.CustomFieldValues.Remove(existingValue);
            }
        }

        foreach (var requestedCustomField in requestedCustomFields)
        {
            var key = requestedCustomField.Key.Trim();
            var customField = customFieldsByName[key];
            var isValid = CustomFieldValueConverter.TryMapToStorage(
                customField.DataType,
                requestedCustomField.Value,
                out var stringValue,
                out var integerValue,
                out var booleanValue);

            if (!isValid)
            {
                return new InvalidRequestException($"Invalid value for custom field '{key}' on contact '{requestContact.Email}'.");
            }

            var existingValue = contact.CustomFieldValues
                .SingleOrDefault(value => value.CustomFieldId == customField.Id);

            if (requestedCustomField.Value.ValueKind == JsonValueKind.Null)
            {
                if (existingValue is not null)
                {
                    dbContext.ContactCustomFieldValues.Remove(existingValue);
                    contact.CustomFieldValues.Remove(existingValue);
                }

                continue;
            }

            if (existingValue is null)
            {
                existingValue = new ContactCustomFieldValue
                {
                    ContactId = contact.Id,
                    CustomFieldId = customField.Id,
                    Contact = contact,
                    CustomField = customField
                };

                contact.CustomFieldValues.Add(existingValue);
                dbContext.ContactCustomFieldValues.Add(existingValue);
            }

            existingValue.StringValue = stringValue;
            existingValue.IntegerValue = integerValue;
            existingValue.BooleanValue = booleanValue;
        }

        return null;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim();
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
