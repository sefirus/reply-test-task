using Microsoft.EntityFrameworkCore;
using Reply.ContactManagement.Core.Entities;

namespace Reply.ContactManagement.Db;

public class ContactManagementDbContext(DbContextOptions<ContactManagementDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<CustomField> CustomFields => Set<CustomField>();

    public DbSet<ContactCustomFieldValue> ContactCustomFieldValues => Set<ContactCustomFieldValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContactManagementDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
