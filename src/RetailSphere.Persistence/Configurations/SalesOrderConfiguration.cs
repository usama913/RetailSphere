using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Persistence.Configurations;

public sealed class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.OrderNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(s => s.OrderNumber).IsUnique();

        builder.Property(s => s.BranchId);
        builder.Property(s => s.CustomerId);
        builder.Property(s => s.CashierUserId);

        builder.Property(s => s.Status).HasMaxLength(20).HasDefaultValue("Completed").IsRequired();
        builder.Property(s => s.OrderDate);
        builder.Property(s => s.PaymentMethod).HasMaxLength(20).HasDefaultValue("Cash").IsRequired();
        builder.Property(s => s.OrderDiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(s => s.AmountPaid).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(s => s.Notes).HasMaxLength(2000);
        builder.Property(s => s.CancellationReason).HasMaxLength(500);

        // Computed, not stored.
        builder.Ignore(s => s.SubtotalAmount);
        builder.Ignore(s => s.LinesDiscountTotal);
        builder.Ignore(s => s.TaxAmount);
        builder.Ignore(s => s.TotalAmount);
        builder.Ignore(s => s.ChangeDue);

        builder.HasMany(s => s.Lines)
            .WithOne()
            .HasForeignKey(l => l.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(s => s.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_lines");

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

public sealed class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable("SalesOrderLines");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.SalesOrderId);
        builder.Property(l => l.ProductId);
        builder.Property(l => l.ProductVariantId);

        builder.Property(l => l.SkuSnapshot).HasMaxLength(64).IsRequired();
        builder.Property(l => l.DescriptionSnapshot).HasMaxLength(500).IsRequired();

        builder.Property(l => l.Quantity).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(l => l.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(l => l.TaxRateSnapshot).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
        builder.Property(l => l.TaxTypeSnapshot).HasMaxLength(20).HasDefaultValue("Exclusive").IsRequired();
        builder.Property(l => l.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(l => l.CostPriceSnapshot).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(l => l.QuantityReturned).HasColumnType("decimal(18,3)").HasDefaultValue(0m);

        // Computed, not stored.
        builder.Ignore(l => l.TaxAmount);
        builder.Ignore(l => l.LineTotal);
    }
}
