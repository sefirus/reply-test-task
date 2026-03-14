using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reply.ContactManagement.Core.Entities;

namespace Reply.ContactManagement.Db.Configurations;

public class ContactCustomFieldValueConfiguration : IEntityTypeConfiguration<ContactCustomFieldValue>
{
    public void Configure(EntityTypeBuilder<ContactCustomFieldValue> builder)
    {
        builder.ToTable("ContactCustomFieldValues");

        builder.HasKey(value => new { value.ContactId, value.CustomFieldId });

        builder.Property(value => value.StringValue)
            .HasMaxLength(16000);

        builder.Property(value => value.IntegerValue);

        builder.Property(value => value.BooleanValue);
    }
}
