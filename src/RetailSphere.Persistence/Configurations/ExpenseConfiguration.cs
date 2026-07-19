using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Finance;

namespace RetailSphere.Persistence.Configurations;

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.BranchId);
        builder.Property(e => e.ExpenseDate);
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.Category).HasMaxLength(50).HasDefaultValue("Other").IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.PaidFromCash).HasDefaultValue(true);
        builder.Property(e => e.RecordedByUserId);

        builder.Property(e => e.CreatedAtUtc);
        builder.Property(e => e.CreatedBy);
        builder.Property(e => e.ModifiedAtUtc);
        builder.Property(e => e.ModifiedBy);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.Property(e => e.DeletedAtUtc);
        builder.Property(e => e.DeletedBy);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.DomainEvents);
    }
}
