using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Persistence.Common;

namespace RetailSphere.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();
        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.IsSystemRole);

        var permissionIdsProperty = builder.Property<List<long>>("_permissionIds")
            .HasField("_permissionIds")
            .UsePropertyAccessMode(Microsoft.EntityFrameworkCore.PropertyAccessMode.Field)
            .HasConversion(JsonLongListConverter.Converter)
            .HasColumnName("PermissionIdsJson")
            .HasColumnType("json");
        permissionIdsProperty.Metadata.SetValueComparer(JsonLongListConverter.Comparer);

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
