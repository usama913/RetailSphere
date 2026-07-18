using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string? Description,
    IReadOnlyList<long> PermissionIds) : IRequest<Result<RoleDto>>;
