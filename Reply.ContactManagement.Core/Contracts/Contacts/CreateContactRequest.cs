namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record CreateContactRequest(
    string Name,
    string? Email,
    string? PhoneNumber,
    DateOnly? Birthday);
