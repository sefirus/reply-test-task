using System.Text.Json;

namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record ContactCustomFieldValueResponse(
    string Key,
    JsonElement Value);
