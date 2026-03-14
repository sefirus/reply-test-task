using Reply.ContactManagement.Core.Enums;

namespace Reply.ContactManagement.Core.Entities;

public class CustomField
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public CustomFieldDataType DataType { get; set; }

    public ICollection<ContactCustomFieldValue> ContactValues { get; set; } = [];
}
