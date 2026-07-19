using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Persistence.Configurations;

public sealed class SalesReturnConfiguration : IEntityTypeConfiguration<SalesReturn>
{
    public void Configure(EntityTypeBuilder<SalesReturn> builder)
    {
        builder.ToTable("SalesReturns");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.ReturnNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(r => r.ReturnNumber).IsUnique();

        builder.Property(r => r.SalesOrderId);
        builder.Property(r => r.BranchId);
        builder.Property(r => r.CustomerId);
        builder.Property(r => r.ProcessedByUserId);
        builder.Property(r => r.ReturnDate);
        builder.Property(r => r.Reason).HasMaxLength(1000);

        // Computed, not stored.
        builder.Ignore(r => r.RefundAmount);

        builder.HasMany(r => r.Lines)
            .WithOne()
            .HasForeignKey(l => l.SalesReturnId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_lines");

        builder.Property(r => r.CreatedAtUtc);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.ModifiedAtUtc);
        builder.Property(r => r.ModifiedBy);
        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.Property(r => r.DeletedAtUtc);
        builder.Property(r => r.DeletedBy);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Ignore(r => r.DomainEvents);
    }
}

public sealed class SalesReturnLineConfiguration : IEntityTypeConfiguration<SalesReturnLine>
{
    public void Configure(EntityTypeBuilder<SalesReturnLine> builder)
    {
        builder.ToTable("SalesReturnLines");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.SalesReturnId);
        builder.Property(l => l.SalesOrderLineId);
        builder.Property(l => l.ProductId);
        builder.Property(l => l.ProductVariantId);

        builder.Property(l => l.SkuSnapshot).HasMaxLength(64).IsRequired();
        builder.Property(l => l.DescriptionSnapshot).HasMaxLength(500).IsRequired();

        builder.Property(l => l.Quantity).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(l => l.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(l => l.TaxRateSnapshot).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
        builder.Property(l => l.TaxTypeSnapshot).HasMaxLength(20).HasDefaultValue("Exclusive").IsRequired();
        builder.Property(l => l.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

        // Computed, not stored.
        builder.Ignore(l => l.TaxAmount);
        builder.Ignore(l => l.RefundAmount);
    }
}
