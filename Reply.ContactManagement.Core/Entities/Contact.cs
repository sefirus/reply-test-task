namespace Reply.ContactManagement.Core.Entities;

public class Contact
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? Birthday { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ICollection<ContactCustomFieldValue> CustomFieldValues { get; set; } = [];
}
