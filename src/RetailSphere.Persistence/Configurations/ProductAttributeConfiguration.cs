using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Configurations;

public sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(a => a.Name).IsUnique();

        builder.HasMany(a => a.Values)
            .WithOne()
            .HasForeignKey(v => v.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Values)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_values");

        builder.Property(a => a.CreatedAtUtc);
        builder.Property(a => a.CreatedBy);
        builder.Property(a => a.ModifiedAtUtc);
        builder.Property(a => a.ModifiedBy);
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);
        builder.Property(a => a.DeletedAtUtc);
        builder.Property(a => a.DeletedBy);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.DomainEvents);
    }
}

public sealed class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
{
    public void Configure(EntityTypeBuilder<AttributeValue> builder)
    {
        builder.ToTable("AttributeValues");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();

        builder.Property(v => v.ProductAttributeId);
        builder.Property(v => v.Value).HasMaxLength(100).IsRequired();
        builder.Property(v => v.DisplayOrder).HasDefaultValue(0);
    }
}
