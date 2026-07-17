using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Persistence.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
        builder.Property(b => b.Code).HasMaxLength(20).IsRequired();
        builder.HasIndex(b => b.Code).IsUnique();

        builder.Property(b => b.Address).HasMaxLength(500);
        builder.Property(b => b.City).HasMaxLength(100);
        builder.Property(b => b.TaxJurisdictionId);
        builder.Property(b => b.CurrencyCode).HasMaxLength(3).IsRequired().HasDefaultValue("PKR");
        builder.Property(b => b.IsActive).HasDefaultValue(true);

        builder.Property(b => b.CreatedAtUtc);
        builder.Property(b => b.CreatedBy);
        builder.Property(b => b.ModifiedAtUtc);
        builder.Property(b => b.ModifiedBy);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAtUtc);
        builder.Property(b => b.DeletedBy);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Ignore(b => b.DomainEvents);
    }
}
