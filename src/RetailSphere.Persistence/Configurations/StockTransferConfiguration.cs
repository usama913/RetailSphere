using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Inventory;

namespace RetailSphere.Persistence.Configurations;

public sealed class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.ToTable("StockTransfers");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.TransferNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(t => t.TransferNumber).IsUnique();

        builder.Property(t => t.FromBranchId);
        builder.Property(t => t.ToBranchId);

        builder.Property(t => t.Status).HasMaxLength(20).HasDefaultValue("Draft").IsRequired();

        builder.Property(t => t.TransferDate);
        builder.Property(t => t.Notes).HasMaxLength(2000);

        // Computed, not stored.
        builder.Ignore(t => t.TotalQuantityRequested);

        builder.HasMany(t => t.Lines)
            .WithOne()
            .HasForeignKey(l => l.StockTransferId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_lines");

        builder.Property(t => t.CreatedAtUtc);
        builder.Property(t => t.CreatedBy);
        builder.Property(t => t.ModifiedAtUtc);
        builder.Property(t => t.ModifiedBy);
        builder.Property(t => t.IsDeleted).HasDefaultValue(false);
        builder.Property(t => t.DeletedAtUtc);
        builder.Property(t => t.DeletedBy);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Ignore(t => t.DomainEvents);
    }
}

public sealed class StockTransferLineConfiguration : IEntityTypeConfiguration<StockTransferLine>
{
    public void Configure(EntityTypeBuilder<StockTransferLine> builder)
    {
        builder.ToTable("StockTransferLines");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.StockTransferId);
        builder.Property(l => l.ProductId);
        builder.Property(l => l.ProductVariantId);

        builder.Property(l => l.SkuSnapshot).HasMaxLength(64).IsRequired();
        builder.Property(l => l.DescriptionSnapshot).HasMaxLength(500).IsRequired();

        builder.Property(l => l.QuantityRequested).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(l => l.QuantityReceived).HasColumnType("decimal(18,3)").HasDefaultValue(0m);

        // Computed, not stored.
        builder.Ignore(l => l.RemainingQuantity);
        builder.Ignore(l => l.IsFullyReceived);
    }
}
