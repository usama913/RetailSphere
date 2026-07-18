using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.ContactPerson).HasMaxLength(150);
        builder.Property(s => s.Email).HasMaxLength(200);
        builder.Property(s => s.Phone).HasMaxLength(50);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.TaxNumber).HasMaxLength(50);

        builder.Property(s => s.IsActive).HasDefaultValue(true);

        builder.Property(s => s.CreatedAtUtc);
        builder.Property(s => s.CreatedBy);
        builder.Property(s => s.ModifiedAtUtc);
        builder.Property(s => s.ModifiedBy);
        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
        builder.Property(s => s.DeletedAtUtc);
        builder.Property(s => s.DeletedBy);

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.DomainEvents);
    }
}
