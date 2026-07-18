using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.AssignRoles;

public sealed record AssignRolesCommand(long UserId, IReadOnlyList<long> RoleIds) : IRequest<Result<UserDto>>;
