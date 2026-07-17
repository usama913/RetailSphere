using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace RetailSphere.API.Authorization;

/// <summary>
/// Permission-based authorization (§7): policies are not pre-registered one by
/// one — any `[Authorize(Policy = "inventory.transfer.approve")]` attribute is
/// satisfied dynamically by checking the caller's "permission" claims for that
/// exact code. Roles are just how permissions get bundled to a user; the checks
/// themselves are always against permission codes.
/// </summary>
public sealed class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim("permission", requirement.PermissionCode))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Every policy name IS the permission code it checks — no manual
        // registration needed as new permissions are added across modules.
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
