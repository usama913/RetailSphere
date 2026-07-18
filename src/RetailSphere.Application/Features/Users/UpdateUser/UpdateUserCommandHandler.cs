using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Users.Common;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    UserDtoAssembler userDtoAssembler)
    : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("User.NotFound", "User not found."));

        var updateResult = user.UpdateProfile(request.FirstName, request.LastName, request.BranchId);
        if (updateResult.IsFailure)
            return Result.Failure<UserDto>(updateResult.Error);

        userRepository.Update(user);
        auditLogService.Log("User", user.Id.ToString(), "Updated", $"Updated profile for '{user.Email.Value}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await userDtoAssembler.ToDtoAsync(user, cancellationToken);
        return Result.Success(dto);
    }
}
