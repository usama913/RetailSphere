using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Catalog;
using RetailSphere.Persistence.Common;

namespace RetailSphere.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(2000);

        // Plain columns, no EF-level FK — Category/Brand/Unit are independent
        // aggregates (see the class remarks on Product for why).
        builder.Property(p => p.CategoryId);
        builder.Property(p => p.BrandId);
        builder.Property(p => p.UnitId);

        builder.Property(p => p.ManageStock).HasDefaultValue(true);
        builder.Property(p => p.NotForSelling).HasDefaultValue(false);

        builder.Property(p => p.IsActive).HasDefaultValue(true);

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Variants)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_variants");

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Images)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_images");

        builder.Property(p => p.CreatedAtUtc);
        builder.Property(p => p.CreatedBy);
        builder.Property(p => p.ModifiedAtUtc);
        builder.Property(p => p.ModifiedBy);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.DeletedAtUtc);
        builder.Property(p => p.DeletedBy);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Ignore(p => p.DomainEvents);
    }
}

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();

        builder.Property(v => v.ProductId);

        builder.Property(v => v.Sku).HasColumnName("Sku").HasMaxLength(64).IsRequired();
        builder.HasIndex(v => v.Sku).IsUnique();

        builder.Property(v => v.Barcode).HasColumnName("Barcode").HasMaxLength(64);
        builder.Property(v => v.BarcodeType).HasMaxLength(20).HasDefaultValue("C128").IsRequired();

        builder.Property(v => v.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(v => v.CompareAtPrice).HasColumnType("decimal(18,2)");
        builder.Property(v => v.CostPrice).HasColumnType("decimal(18,2)");
        builder.Property(v => v.TaxRate).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
        builder.Property(v => v.TaxType).HasMaxLength(20).HasDefaultValue("Exclusive").IsRequired();
        builder.Property(v => v.Weight).HasColumnType("decimal(10,3)");
        builder.Property(v => v.Length).HasColumnType("decimal(10,3)");
        builder.Property(v => v.Width).HasColumnType("decimal(10,3)");
        builder.Property(v => v.Height).HasColumnType("decimal(10,3)");
        builder.Property(v => v.ReorderPoint).HasColumnType("decimal(18,3)");
        builder.Property(v => v.ExpiryDate);

        builder.Property(v => v.IsActive).HasDefaultValue(true);

        var attributeValueIdsProperty = builder.Property<List<long>>("_attributeValueIds")
            .HasField("_attributeValueIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(JsonLongListConverter.Converter)
            .HasColumnName("AttributeValueIdsJson")
            .HasColumnType("json");
        attributeValueIdsProperty.Metadata.SetValueComparer(JsonLongListConverter.Comparer);
    }
}

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.ProductId);
        builder.Property(i => i.Url).HasMaxLength(1000).IsRequired();
        builder.Property(i => i.DisplayOrder).HasDefaultValue(0);
    }
}
