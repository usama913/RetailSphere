using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.GetUserById;

public sealed record GetUserByIdQuery(long Id) : IRequest<Result<UserDto>>;
