namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record UpdateContactRequest(
    string Name,
    string? Email,
    string? PhoneNumber,
    DateOnly? Birthday);
