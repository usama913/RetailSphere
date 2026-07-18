using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RetailSphere.Common;

namespace RetailSphere.Infrastructure.Security;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public long? UserId
    {
        get
        {
            var value = User?.FindFirstValue("sub") ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("email");

    public long? BranchId
    {
        get
        {
            var value = User?.FindFirstValue("branch_id");
            return long.TryParse(value, out var id) ? id : null;
        }
    }

    public IReadOnlyCollection<string> Permissions =>
        User?.FindAll("permission").Select(c => c.Value).ToList() ?? [];

    public bool HasPermission(string permissionCode) => Permissions.Contains(permissionCode);
}
