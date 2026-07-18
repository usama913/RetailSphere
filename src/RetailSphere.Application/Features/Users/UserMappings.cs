using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;

namespace RetailSphere.Application.Features.Users;

internal static class UserMappings
{
    public static UserDto ToDto(User user, string? branchName, IReadOnlyList<string> roleNames) => new()
    {
        Id = user.Id,
        Email = user.Email.Value,
        FirstName = user.FirstName,
        LastName = user.LastName,
        BranchId = user.BranchId,
        BranchName = branchName,
        IsActive = user.IsActive,
        LastLoginAtUtc = user.LastLoginAtUtc,
        RoleIds = user.RoleIds.ToList(),
        RoleNames = roleNames,
    };
}
