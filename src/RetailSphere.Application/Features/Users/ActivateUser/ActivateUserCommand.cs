using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.ActivateUser;

public sealed record ActivateUserCommand(long Id) : IRequest<Result>;
