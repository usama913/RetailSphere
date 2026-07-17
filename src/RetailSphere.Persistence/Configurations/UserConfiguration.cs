using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using RetailSphere.Persistence.Common;

namespace RetailSphere.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property<Guid>("PublicId").HasDefaultValueSql("(UUID())").ValueGeneratedOnAdd();
        builder.HasIndex("PublicId").IsUnique();

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .HasColumnName("Email")
            .HasMaxLength(256)
            .IsRequired();

        // Plain string column used for all querying (see the XML doc on
        // User.NormalizedEmail for why) — this is the one with the unique index,
        // not the value-converted Email property above.
        builder.Property(u => u.NormalizedEmail)
            .HasColumnName("NormalizedEmail")
            .HasMaxLength(256)
            .IsRequired();
        builder.HasIndex(u => u.NormalizedEmail).IsUnique();

        builder.Property(u => u.Password)
            .HasConversion(password => password.Value, value => HashedPassword.FromHash(value).Value)
            .HasColumnName("PasswordHash")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.BranchId);
        builder.Property(u => u.IsActive);
        builder.Property(u => u.LastLoginAtUtc);

        // Backing field only — RoleIds is exposed as a read-only collection on the
        // aggregate. See JsonLongListConverter for why this is a JSON column for now.
        var roleIdsProperty = builder.Property<List<long>>("_roleIds")
            .HasField("_roleIds")
            .UsePropertyAccessMode(Microsoft.EntityFrameworkCore.PropertyAccessMode.Field)
            .HasConversion(JsonLongListConverter.Converter)
            .HasColumnName("RoleIdsJson")
            .HasColumnType("json");
        roleIdsProperty.Metadata.SetValueComparer(JsonLongListConverter.Comparer);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.RefreshTokens)
            .UsePropertyAccessMode(Microsoft.EntityFrameworkCore.PropertyAccessMode.Field)
            .HasField("_refreshTokens");

        // Audit + soft delete
        builder.Property(u => u.CreatedAtUtc);
        builder.Property(u => u.CreatedBy);
        builder.Property(u => u.ModifiedAtUtc);
        builder.Property(u => u.ModifiedBy);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
        builder.Property(u => u.DeletedAtUtc);
        builder.Property(u => u.DeletedBy);

        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.Ignore(u => u.DomainEvents);
    }
}
