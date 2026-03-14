using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reply.ContactManagement.Core.Entities;

namespace Reply.ContactManagement.Db.Configurations;

public class CustomFieldConfiguration : IEntityTypeConfiguration<CustomField>
{
    public void Configure(EntityTypeBuilder<CustomField> builder)
    {
        builder.ToTable("CustomFields");

        builder.HasKey(customField => customField.Id);

        builder.Property(customField => customField.Name)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(customField => customField.DataType)
            .IsRequired();

        builder.HasIndex(customField => customField.Name)
            .IsUnique();

        builder.HasMany(customField => customField.ContactValues)
            .WithOne(value => value.CustomField)
            .HasForeignKey(value => value.CustomFieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
