using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.DeactivateUser;

public sealed record DeactivateUserCommand(long Id) : IRequest<Result>;
