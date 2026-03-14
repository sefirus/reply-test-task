using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reply.ContactManagement.Core.Entities;

namespace Reply.ContactManagement.Db.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Name)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(contact => contact.Email)
            .HasMaxLength(400);

        builder.Property(contact => contact.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(contact => contact.Birthday);

        builder.Property(contact => contact.CreatedAtUtc)
            .IsRequired();

        builder.Property(contact => contact.UpdatedAtUtc)
            .IsRequired();

        builder.HasMany(contact => contact.CustomFieldValues)
            .WithOne(value => value.Contact)
            .HasForeignKey(value => value.ContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
