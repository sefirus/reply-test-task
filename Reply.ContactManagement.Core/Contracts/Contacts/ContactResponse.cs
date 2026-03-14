namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record ContactResponse(
    Guid Id,
    string Name,
    string? Email,
    string? PhoneNumber,
    DateOnly? Birthday,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyCollection<ContactCustomFieldValueResponse> CustomFields);
