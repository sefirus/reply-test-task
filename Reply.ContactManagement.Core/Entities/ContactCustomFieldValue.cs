namespace Reply.ContactManagement.Core.Entities;

public class ContactCustomFieldValue
{
    public Guid ContactId { get; set; }

    public Guid CustomFieldId { get; set; }

    public string? StringValue { get; set; }

    public int? IntegerValue { get; set; }

    public bool? BooleanValue { get; set; }

    public Contact Contact { get; set; } = null!;

    public CustomField CustomField { get; set; } = null!;
}
