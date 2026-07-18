using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess;

/// <summary>
/// A named, user-manageable bundle of permissions (e.g. "Branch Manager").
/// Authorization checks are always against permission codes, never role names
/// directly, so new roles can be composed without a code change.
/// </summary>
public sealed class Role : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    private readonly List<long> _permissionIds = [];

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public bool IsSystemRole { get; private set; }

    public IReadOnlyCollection<long> PermissionIds => _permissionIds.AsReadOnly();

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Role()
    {
    }

    public static Result<Role> Create(string name, string? description, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Role>(Error.Validation("Role.NameRequired", "Role name is required."));

        var role = new Role
        {
            Name = name.Trim(),
            Description = description,
            IsSystemRole = isSystemRole,
        };

        return Result.Success(role);
    }

    public Result UpdateDetails(string name, string? description)
    {
        if (IsSystemRole)
            return Result.Failure(Error.Failure("Role.SystemRoleImmutable", "System roles cannot be modified."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Role.NameRequired", "Role name is required."));

        Name = name.Trim();
        Description = description;
        return Result.Success();
    }

    public Result GrantPermission(long permissionId)
    {
        if (_permissionIds.Contains(permissionId))
            return Result.Failure(Error.Conflict("Role.PermissionAlreadyGranted", "The role already has this permission."));

        _permissionIds.Add(permissionId);
        return Result.Success();
    }

    public Result RevokePermission(long permissionId)
    {
        if (IsSystemRole)
            return Result.Failure(Error.Failure("Role.SystemRoleImmutable", "System roles cannot be modified."));

        if (!_permissionIds.Remove(permissionId))
            return Result.Failure(Error.NotFound("Role.PermissionNotGranted", "The role does not have this permission."));

        return Result.Success();
    }

    public bool HasPermission(long permissionId) => _permissionIds.Contains(permissionId);
}
