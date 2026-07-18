using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Configurations;

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        builder.Property(b => b.Name).HasMaxLength(150).IsRequired();
        builder.HasIndex(b => b.Name).IsUnique();
        builder.Property(b => b.Description).HasMaxLength(1000);
        builder.Property(b => b.IsActive).HasDefaultValue(true);

        builder.Property(b => b.CreatedAtUtc);
        builder.Property(b => b.CreatedBy);
        builder.Property(b => b.ModifiedAtUtc);
        builder.Property(b => b.ModifiedBy);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAtUtc);
        builder.Property(b => b.DeletedBy);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Ignore(b => b.DomainEvents);
    }
}
