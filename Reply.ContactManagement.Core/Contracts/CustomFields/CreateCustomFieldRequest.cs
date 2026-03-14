using Reply.ContactManagement.Core.Enums;

namespace Reply.ContactManagement.Core.Contracts.CustomFields;

public sealed record CreateCustomFieldRequest(
    string Name,
    CustomFieldDataType DataType);
