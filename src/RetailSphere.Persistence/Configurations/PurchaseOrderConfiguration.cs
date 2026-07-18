using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Configurations;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.PoNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(p => p.PoNumber).IsUnique();

        builder.Property(p => p.SupplierId);
        builder.Property(p => p.BranchId);

        builder.Property(p => p.Status).HasMaxLength(20).HasDefaultValue("Draft").IsRequired();

        builder.Property(p => p.OrderDate);
        builder.Property(p => p.ExpectedDeliveryDate);
        builder.Property(p => p.Notes).HasMaxLength(2000);

        // Computed, not stored — read-only properties derived from the Lines collection.
        builder.Ignore(p => p.TotalAmount);

        builder.HasMany(p => p.Lines)
            .WithOne()
            .HasForeignKey(l => l.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_lines");

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

public sealed class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("PurchaseOrderLines");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.PurchaseOrderId);
        builder.Property(l => l.ProductId);
        builder.Property(l => l.ProductVariantId);

        builder.Property(l => l.SkuSnapshot).HasMaxLength(64).IsRequired();
        builder.Property(l => l.DescriptionSnapshot).HasMaxLength(500).IsRequired();

        builder.Property(l => l.QuantityOrdered).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(l => l.QuantityReceived).HasColumnType("decimal(18,3)").HasDefaultValue(0m);
        builder.Property(l => l.UnitCost).HasColumnType("decimal(18,2)").IsRequired();

        // Computed, not stored.
        builder.Ignore(l => l.LineTotal);
        builder.Ignore(l => l.RemainingQuantity);
        builder.Ignore(l => l.IsFullyReceived);
    }
}
