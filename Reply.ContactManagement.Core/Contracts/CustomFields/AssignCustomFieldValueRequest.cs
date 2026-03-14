using System.Text.Json;

namespace Reply.ContactManagement.Core.Contracts.CustomFields;

public sealed record AssignCustomFieldValueRequest(
    JsonElement Value);
