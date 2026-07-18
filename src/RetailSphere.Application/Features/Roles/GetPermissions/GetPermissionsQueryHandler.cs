using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.GetPermissions;

public sealed class GetPermissionsQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
{
    public async Task<Result<IReadOnlyList<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await roleRepository.GetAllPermissionsAsync(cancellationToken);
        var dtos = permissions.Select(RoleMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<PermissionDto>>(dtos);
    }
}
