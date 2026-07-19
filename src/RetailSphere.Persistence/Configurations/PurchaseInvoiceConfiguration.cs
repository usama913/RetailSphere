using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Configurations;

public sealed class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.ToTable("PurchaseInvoices");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.SupplierId);
        builder.Property(i => i.BranchId);
        builder.Property(i => i.PurchaseOrderId);

        builder.Property(i => i.SupplierInvoiceNumber).HasMaxLength(60).IsRequired();
        builder.HasIndex(i => new { i.SupplierId, i.SupplierInvoiceNumber });

        builder.Property(i => i.InvoiceDate);
        builder.Property(i => i.DueDate);
        builder.Property(i => i.PaymentTerms).HasMaxLength(30).HasDefaultValue("Cash").IsRequired();

        builder.Property(i => i.SubtotalAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.AmountPaid).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

        builder.Property(i => i.Notes).HasMaxLength(2000);

        // Computed, not stored.
        builder.Ignore(i => i.TotalAmount);
        builder.Ignore(i => i.OutstandingBalance);
        builder.Ignore(i => i.PaymentStatus);

        builder.Property(i => i.CreatedAtUtc);
        builder.Property(i => i.CreatedBy);
        builder.Property(i => i.ModifiedAtUtc);
        builder.Property(i => i.ModifiedBy);
        builder.Property(i => i.IsDeleted).HasDefaultValue(false);
        builder.Property(i => i.DeletedAtUtc);
        builder.Property(i => i.DeletedBy);

        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.Ignore(i => i.DomainEvents);
    }
}
