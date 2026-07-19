using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.Notifications;

namespace RetailSphere.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedOnAdd();

        builder.Property(n => n.Type).HasMaxLength(60).IsRequired();
        builder.Property(n => n.Severity).HasMaxLength(20).HasDefaultValue("Info").IsRequired();
        builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.RelatedEntityType).HasMaxLength(60);
        builder.Property(n => n.RelatedEntityId);
        builder.Property(n => n.UserId);

        builder.Property(n => n.IsRead).HasDefaultValue(false);
        builder.Property(n => n.ReadAtUtc);

        builder.Property(n => n.EmailSent).HasDefaultValue(false);
        builder.Property(n => n.EmailSentAtUtc);

        builder.Property(n => n.CreatedAtUtc);

        builder.HasIndex(n => new { n.UserId, n.IsRead });
        builder.HasIndex(n => new { n.RelatedEntityType, n.RelatedEntityId });
    }
}
