using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.DeleteRole;

public sealed record DeleteRoleCommand(long Id) : IRequest<Result>;
