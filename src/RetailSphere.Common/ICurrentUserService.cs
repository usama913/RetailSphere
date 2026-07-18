namespace RetailSphere.Common;

/// <summary>
/// Abstraction over "who is making this request" — implemented in the API layer
/// from the JWT claims principal, so Application/Persistence never touch
/// HttpContext directly (keeps them testable and framework-agnostic).
/// </summary>
public interface ICurrentUserService
{
    long? UserId { get; }

    string? Email { get; }

    long? BranchId { get; }

    IReadOnlyCollection<string> Permissions { get; }

    bool IsAuthenticated { get; }

    bool HasPermission(string permissionCode);
}
