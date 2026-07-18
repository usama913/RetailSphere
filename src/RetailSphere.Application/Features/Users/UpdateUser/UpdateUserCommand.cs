using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(
    long Id,
    string FirstName,
    string LastName,
    long? BranchId) : IRequest<Result<UserDto>>;
