using Reply.ContactManagement.Core.Enums;

namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed class GetContactsRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public string? Email { get; init; }

    public string? PhoneNumber { get; init; }

    public DateOnly? BirthdayFrom { get; init; }

    public DateOnly? BirthdayTo { get; init; }

    public DateTimeOffset? CreatedFromUtc { get; init; }

    public DateTimeOffset? CreatedToUtc { get; init; }

    public ContactSortBy SortBy { get; init; } = ContactSortBy.CreatedAtUtc;

    public SortDirection SortDirection { get; init; } = SortDirection.Desc;
}
