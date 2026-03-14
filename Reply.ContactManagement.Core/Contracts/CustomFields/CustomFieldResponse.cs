using Reply.ContactManagement.Core.Enums;

namespace Reply.ContactManagement.Core.Contracts.CustomFields;

public sealed record CustomFieldResponse(
    Guid Id,
    string Name,
    CustomFieldDataType DataType);
