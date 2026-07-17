using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess;

/// <summary>
/// A single, granular, checkable permission (e.g. "inventory.transfer.approve").
/// Permissions are seeded reference data, not user-manageable — roles are the
/// user-manageable bundles of permissions (see <see cref="Role"/>).
/// </summary>
public sealed class Permission : Entity<long>
{
    public string Code { get; private set; } = default!;

    public string DisplayName { get; private set; } = default!;

    public string Module { get; private set; } = default!;

    private Permission()
    {
    }

    public static Permission Create(long id, string code, string displayName, string module) => new()
    {
        Id = id,
        Code = code,
        DisplayName = displayName,
        Module = module,
    };
}
