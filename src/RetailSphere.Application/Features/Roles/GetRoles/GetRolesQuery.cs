using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyList<RoleDto>>>;
