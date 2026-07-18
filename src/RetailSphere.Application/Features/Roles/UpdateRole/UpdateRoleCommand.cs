using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.UpdateRole;

public sealed record UpdateRoleCommand(
    long Id,
    string Name,
    string? Description,
    IReadOnlyList<long> PermissionIds) : IRequest<Result<RoleDto>>;
