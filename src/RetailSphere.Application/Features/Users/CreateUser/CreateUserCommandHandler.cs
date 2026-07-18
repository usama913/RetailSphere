using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Users.Common;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    UserDtoAssembler userDtoAssembler)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.EmailExistsAsync(request.Email, cancellationToken))
            return Result.Failure<UserDto>(Error.Conflict("User.EmailExists", "A user with this email already exists."));

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<UserDto>(emailResult.Error);

        var hashedPasswordResult = HashedPassword.FromHash(passwordHasher.Hash(request.Password));
        if (hashedPasswordResult.IsFailure)
            return Result.Failure<UserDto>(hashedPasswordResult.Error);

        var userResult = User.Register(emailResult.Value, hashedPasswordResult.Value, request.FirstName, request.LastName, request.BranchId);
        if (userResult.IsFailure)
            return Result.Failure<UserDto>(userResult.Error);

        var user = userResult.Value;
        foreach (var roleId in request.RoleIds.Distinct())
        {
            user.AssignRole(roleId);
        }

        userRepository.Add(user);

        // Save once first so the auto-increment Id exists before it's referenced in the audit entry.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("User", user.Id.ToString(), "Created", $"Created user '{user.Email.Value}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await userDtoAssembler.ToDtoAsync(user, cancellationToken);
        return Result.Success(dto);
    }
}
