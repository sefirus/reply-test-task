using Microsoft.EntityFrameworkCore;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Core.Contracts.Common;
using Reply.ContactManagement.Core.Contracts.Contacts;
using Reply.ContactManagement.Core.Entities;
using Reply.ContactManagement.Core.Enums;
using Reply.ContactManagement.Core.Exceptions;
using Reply.ContactManagement.Db;

namespace Reply.ContactManagement.Application.Services;

public sealed class ContactService(ContactManagementDbContext dbContext) : IContactService
{
    public async Task<CreatedEntityResponse> CreateAsync(CreateContactRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = NormalizeNullable(request.Email),
            PhoneNumber = NormalizeNullable(request.PhoneNumber),
            Birthday = request.Birthday,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatedEntityResponse(contact.Id);
    }

    public async Task<PagedResponse<ContactResponse>> GetAsync(GetContactsRequest request, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Contacts
            .AsNoTracking();

        query = ApplyFilters(query, request);
        query = ApplySorting(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        var contacts = await query
            .Include(contact => contact.CustomFieldValues)
            .ThenInclude(value => value.CustomField)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = contacts
            .Select(MapToResponse)
            .ToArray();

        return new PagedResponse<ContactResponse>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<ContactResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await dbContext.Contacts
            .AsNoTracking()
            .Include(value => value.CustomFieldValues)
            .ThenInclude(value => value.CustomField)
            .SingleOrDefaultAsync(contact => contact.Id == id, cancellationToken);

        return contact is null
            ? throw new EntityNotFoundException("Contact", id)
            : MapToResponse(contact);
    }

    public async Task<ContactResponse> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken cancellationToken = default)
    {
        var contact = await dbContext.Contacts
            .SingleOrDefaultAsync(currentContact => currentContact.Id == id, cancellationToken);

        if (contact is null)
        {
            throw new EntityNotFoundException("Contact", id);
        }

        contact.Name = request.Name.Trim();
        contact.Email = NormalizeNullable(request.Email);
        contact.PhoneNumber = NormalizeNullable(request.PhoneNumber);
        contact.Birthday = request.Birthday;
        contact.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deletedCount = await dbContext.Contacts
            .Where(contact => contact.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount == 0)
        {
            throw new EntityNotFoundException("Contact", id);
        }
    }

    private static IQueryable<Contact> ApplyFilters(IQueryable<Contact> query, GetContactsRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(contact =>
                contact.Name.Contains(search) ||
                (contact.Email != null && contact.Email.Contains(search)) ||
                (contact.PhoneNumber != null && contact.PhoneNumber.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            query = query.Where(contact => contact.Email != null && contact.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneNumber = request.PhoneNumber.Trim();
            query = query.Where(contact => contact.PhoneNumber != null && contact.PhoneNumber.Contains(phoneNumber));
        }

        if (request.BirthdayFrom.HasValue)
        {
            query = query.Where(contact => contact.Birthday >= request.BirthdayFrom.Value);
        }

        if (request.BirthdayTo.HasValue)
        {
            query = query.Where(contact => contact.Birthday <= request.BirthdayTo.Value);
        }

        if (request.CreatedFromUtc.HasValue)
        {
            query = query.Where(contact => contact.CreatedAtUtc >= request.CreatedFromUtc.Value);
        }

        if (request.CreatedToUtc.HasValue)
        {
            query = query.Where(contact => contact.CreatedAtUtc <= request.CreatedToUtc.Value);
        }

        return query;
    }

    private static IQueryable<Contact> ApplySorting(IQueryable<Contact> query, GetContactsRequest request)
    {
        return (request.SortBy, request.SortDirection) switch
        {
            (ContactSortBy.Name, SortDirection.Asc) => query.OrderBy(contact => contact.Name).ThenBy(contact => contact.CreatedAtUtc),
            (ContactSortBy.Name, SortDirection.Desc) => query.OrderByDescending(contact => contact.Name).ThenByDescending(contact => contact.CreatedAtUtc),
            (ContactSortBy.Email, SortDirection.Asc) => query.OrderBy(contact => contact.Email).ThenBy(contact => contact.CreatedAtUtc),
            (ContactSortBy.Email, SortDirection.Desc) => query.OrderByDescending(contact => contact.Email).ThenByDescending(contact => contact.CreatedAtUtc),
            (ContactSortBy.UpdatedAtUtc, SortDirection.Asc) => query.OrderBy(contact => contact.UpdatedAtUtc).ThenBy(contact => contact.CreatedAtUtc),
            (ContactSortBy.UpdatedAtUtc, SortDirection.Desc) => query.OrderByDescending(contact => contact.UpdatedAtUtc).ThenByDescending(contact => contact.CreatedAtUtc),
            (ContactSortBy.CreatedAtUtc, SortDirection.Asc) => query.OrderBy(contact => contact.CreatedAtUtc),
            _ => query.OrderByDescending(contact => contact.CreatedAtUtc)
        };
    }

    private static ContactResponse MapToResponse(Contact contact)
    {
        var customFields = contact.CustomFieldValues
            .OrderBy(value => value.CustomField.Name)
            .Select(value => new ContactCustomFieldValueResponse(
                value.CustomField.Name,
                CustomFieldValueConverter.ToJsonElement(value, value.CustomField.DataType)))
            .ToArray();

        return new ContactResponse(
            contact.Id,
            contact.Name,
            contact.Email,
            contact.PhoneNumber,
            contact.Birthday,
            contact.CreatedAtUtc,
            contact.UpdatedAtUtc,
            customFields);
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
