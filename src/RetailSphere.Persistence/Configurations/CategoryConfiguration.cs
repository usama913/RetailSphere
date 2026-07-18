using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Slug).HasMaxLength(150).IsRequired();
        builder.HasIndex(c => c.Slug);

        // Self-referencing tree — plain nullable column, no EF-level FK, matching
        // this codebase's convention of not wiring cross-aggregate navigations
        // (see Product.CategoryId/BrandId for the same reasoning).
        builder.Property(c => c.ParentCategoryId);

        builder.Property(c => c.IsActive).HasDefaultValue(true);

        builder.Property(c => c.CreatedAtUtc);
        builder.Property(c => c.CreatedBy);
        builder.Property(c => c.ModifiedAtUtc);
        builder.Property(c => c.ModifiedBy);
        builder.Property(c => c.IsDeleted).HasDefaultValue(false);
        builder.Property(c => c.DeletedAtUtc);
        builder.Property(c => c.DeletedBy);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.DomainEvents);
    }
}
