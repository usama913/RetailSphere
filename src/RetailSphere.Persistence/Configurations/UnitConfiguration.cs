using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Configurations;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Name).IsUnique();

        builder.Property(u => u.ShortCode).HasMaxLength(20).IsRequired();

        builder.Property(u => u.AllowDecimal).HasDefaultValue(true);
        builder.Property(u => u.IsActive).HasDefaultValue(true);

        builder.Property(u => u.CreatedAtUtc);
        builder.Property(u => u.CreatedBy);
        builder.Property(u => u.ModifiedAtUtc);
        builder.Property(u => u.ModifiedBy);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
        builder.Property(u => u.DeletedAtUtc);
        builder.Property(u => u.DeletedBy);

        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.Ignore(u => u.DomainEvents);
    }
}
