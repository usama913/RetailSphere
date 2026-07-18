using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.GetRoles;

public sealed class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        var dtos = roles.Select(RoleMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<RoleDto>>(dtos);
    }
}
