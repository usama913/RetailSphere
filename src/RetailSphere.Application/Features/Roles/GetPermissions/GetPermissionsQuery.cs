using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.GetPermissions;

public sealed record GetPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionDto>>>;
