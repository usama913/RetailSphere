using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Auditing;

namespace RetailSphere.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.TimestampUtc).IsRequired();
        builder.HasIndex(a => a.TimestampUtc);

        builder.Property(a => a.UserId);
        builder.Property(a => a.UserEmail).HasMaxLength(256);

        builder.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
        builder.HasIndex(a => a.EntityType);

        builder.Property(a => a.EntityId).HasMaxLength(50).IsRequired();

        builder.Property(a => a.Action).HasMaxLength(50).IsRequired();

        builder.Property(a => a.Details).HasMaxLength(1000);
    }
}
