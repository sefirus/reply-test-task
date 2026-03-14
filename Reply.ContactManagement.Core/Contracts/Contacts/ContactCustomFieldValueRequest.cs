using System.Text.Json;

namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record ContactCustomFieldValueRequest(
    string Key,
    JsonElement Value);
