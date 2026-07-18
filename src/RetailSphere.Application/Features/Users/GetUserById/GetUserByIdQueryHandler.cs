using MediatR;
using RetailSphere.Application.Features.Users.Common;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository, UserDtoAssembler userDtoAssembler)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("User.NotFound", "User not found."));

        var dto = await userDtoAssembler.ToDtoAsync(user, cancellationToken);
        return Result.Success(dto);
    }
}
