namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record BulkMergeContactRequest(
    string Name,
    string Email,
    string? PhoneNumber,
    DateOnly? Birthday,
    IReadOnlyCollection<ContactCustomFieldValueRequest>? CustomFields);
