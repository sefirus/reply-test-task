using Microsoft.EntityFrameworkCore;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Core.Contracts.Common;
using Reply.ContactManagement.Core.Contracts.Contacts;
using Reply.ContactManagement.Core.Contracts.CustomFields;
using Reply.ContactManagement.Core.Entities;
using Reply.ContactManagement.Core.Exceptions;
using Reply.ContactManagement.Db;
using System.Text.Json;

namespace Reply.ContactManagement.Application.Services;

public sealed class CustomFieldService(ContactManagementDbContext dbContext) : ICustomFieldService
{
    public async Task<IReadOnlyCollection<CustomFieldResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.CustomFields
            .AsNoTracking()
            .OrderBy(customField => customField.Name)
            .Select(customField => new CustomFieldResponse(
                customField.Id,
                customField.Name,
                customField.DataType))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<CreatedEntityResponse> CreateAsync(CreateCustomFieldRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();
        var alreadyExists = await dbContext.CustomFields
            .AnyAsync(customField => customField.Name == normalizedName, cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException("A custom field with the same name already exists.");
        }

        var customField = new CustomField
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            DataType = request.DataType
        };

        dbContext.CustomFields.Add(customField);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatedEntityResponse(customField.Id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deletedCount = await dbContext.CustomFields
            .Where(customField => customField.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount == 0)
        {
            throw new EntityNotFoundException("Custom field", id);
        }
    }

    public async Task<ContactCustomFieldValueResponse> AssignValueAsync(
        Guid contactId,
        Guid customFieldId,
        AssignCustomFieldValueRequest request,
        CancellationToken cancellationToken = default)
    {
        var contactExists = await dbContext.Contacts
            .AnyAsync(contact => contact.Id == contactId, cancellationToken);

        if (!contactExists)
        {
            throw new EntityNotFoundException("Contact", contactId);
        }

        var customField = await dbContext.CustomFields
            .AsNoTracking()
            .SingleOrDefaultAsync(field => field.Id == customFieldId, cancellationToken);

        if (customField is null)
        {
            throw new EntityNotFoundException("Custom field", customFieldId);
        }

        var isValidValue = CustomFieldValueConverter.TryMapToStorage(
            customField.DataType,
            request.Value,
            out var stringValue,
            out var integerValue,
            out var booleanValue);

        if (!isValidValue)
        {
            throw new InvalidRequestException($"The value does not match custom field type '{customField.DataType}'.");
        }

        var currentValue = await dbContext.ContactCustomFieldValues
            .SingleOrDefaultAsync(
                value => value.ContactId == contactId && value.CustomFieldId == customFieldId,
                cancellationToken);

        if (request.Value.ValueKind == JsonValueKind.Null)
        {
            if (currentValue is not null)
            {
                dbContext.ContactCustomFieldValues.Remove(currentValue);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return new ContactCustomFieldValueResponse(
                customField.Name,
                JsonSerializer.SerializeToElement<object?>(null));
        }

        if (currentValue is null)
        {
            currentValue = new ContactCustomFieldValue
            {
                ContactId = contactId,
                CustomFieldId = customFieldId
            };

            dbContext.ContactCustomFieldValues.Add(currentValue);
        }

        currentValue.StringValue = stringValue;
        currentValue.IntegerValue = integerValue;
        currentValue.BooleanValue = booleanValue;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ContactCustomFieldValueResponse(
            customField.Name,
            CustomFieldValueConverter.ToJsonElement(currentValue, customField.DataType));
    }
}
