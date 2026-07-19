using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Customers;

namespace RetailSphere.Persistence.Configurations;

public sealed class CustomerPaymentConfiguration : IEntityTypeConfiguration<CustomerPayment>
{
    public void Configure(EntityTypeBuilder<CustomerPayment> builder)
    {
        builder.ToTable("CustomerPayments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.CustomerId);
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

        // Computed, not stored.
        builder.Ignore(p => p.AllocatedAmount);
        builder.Ignore(p => p.UnallocatedAmount);

        builder.HasMany(p => p.Allocations)
            .WithOne()
            .HasForeignKey(a => a.CustomerPaymentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Allocations)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_allocations");

        builder.Property(p => p.CreatedAtUtc);
        builder.Property(p => p.CreatedBy);
        builder.Property(p => p.ModifiedAtUtc);
        builder.Property(p => p.ModifiedBy);

        builder.Ignore(p => p.DomainEvents);
    }
}

public sealed class CustomerPaymentAllocationConfiguration : IEntityTypeConfiguration<CustomerPaymentAllocation>
{
    public void Configure(EntityTypeBuilder<CustomerPaymentAllocation> builder)
    {
        builder.ToTable("CustomerPaymentAllocations");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.CustomerPaymentId);
        builder.Property(a => a.SalesOrderId);
        builder.Property(a => a.Amount).HasColumnType("decimal(18,2)").IsRequired();
    }
}
