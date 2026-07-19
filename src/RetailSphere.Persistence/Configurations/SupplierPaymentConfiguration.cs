using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Configurations;

public sealed class SupplierPaymentConfiguration : IEntityTypeConfiguration<SupplierPayment>
{
    public void Configure(EntityTypeBuilder<SupplierPayment> builder)
    {
        builder.ToTable("SupplierPayments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.SupplierId);
        builder.Property(p => p.PurchaseInvoiceId);
        builder.Property(p => p.BranchId);

        builder.Property(p => p.PaymentDate);
        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.PaymentMethod).HasMaxLength(20).HasDefaultValue("Cash").IsRequired();
        builder.Property(p => p.ReferenceNumber).HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(1000);

        builder.Property(p => p.IsReversed).HasDefaultValue(false);
        builder.Property(p => p.ReversalReason).HasMaxLength(500);
        builder.Property(p => p.ReversedAtUtc);
        builder.Property(p => p.ReversedByUserId);

        builder.Property(p => p.CreatedAtUtc);
        builder.Property(p => p.CreatedBy);
        builder.Property(p => p.ModifiedAtUtc);
        builder.Property(p => p.ModifiedBy);

        builder.Ignore(p => p.DomainEvents);
    }
}
