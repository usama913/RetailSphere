using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Inventory;

namespace RetailSphere.Persistence.Configurations;

public sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("StockItems");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        // Plain columns, no EF navigation — ProductVariant/Branch are independent
        // aggregates (see the class remarks on StockItem).
        builder.Property(s => s.ProductVariantId);
        builder.Property(s => s.BranchId);

        builder.Property(s => s.QuantityOnHand).HasColumnType("decimal(18,3)").HasDefaultValue(0m);

        // Exactly one stock row per variant per branch.
        builder.HasIndex(s => new { s.ProductVariantId, s.BranchId }).IsUnique();

        builder.HasMany(s => s.Adjustments)
            .WithOne()
            .HasForeignKey(a => a.StockItemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(s => s.Adjustments)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_adjustments");

        builder.Property(s => s.CreatedAtUtc);
        builder.Property(s => s.CreatedBy);
        builder.Property(s => s.ModifiedAtUtc);
        builder.Property(s => s.ModifiedBy);

        builder.Ignore(s => s.DomainEvents);
    }
}

public sealed class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("StockAdjustments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.StockItemId);
        builder.Property(a => a.QuantityDelta).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(a => a.Reason).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Source).HasMaxLength(30).HasDefaultValue("Manual").IsRequired();

        builder.Property(a => a.CreatedAtUtc);
        builder.Property(a => a.CreatedBy);
        builder.Property(a => a.ModifiedAtUtc);
        builder.Property(a => a.ModifiedBy);
    }
}
