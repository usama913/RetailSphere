using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Finance;

namespace RetailSphere.Persistence.Configurations;

public sealed class CashRegisterSessionConfiguration : IEntityTypeConfiguration<CashRegisterSession>
{
    public void Configure(EntityTypeBuilder<CashRegisterSession> builder)
    {
        builder.ToTable("CashRegisterSessions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.BranchId);
        builder.Property(s => s.OpenedByUserId);
        builder.Property(s => s.ClosedByUserId);

        builder.Property(s => s.Status).HasMaxLength(20).HasDefaultValue("Open").IsRequired();
        builder.Property(s => s.OpeningBalance).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.ClosingBalance).HasColumnType("decimal(18,2)");
        builder.Property(s => s.OpenedAtUtc);
        builder.Property(s => s.ClosedAtUtc);
        builder.Property(s => s.OpeningNotes).HasMaxLength(1000);
        builder.Property(s => s.ClosingNotes).HasMaxLength(1000);

        builder.Property(s => s.CreatedAtUtc);
        builder.Property(s => s.CreatedBy);
        builder.Property(s => s.ModifiedAtUtc);
        builder.Property(s => s.ModifiedBy);

        builder.Ignore(s => s.DomainEvents);
    }
}
