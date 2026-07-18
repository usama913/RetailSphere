using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Users.Common;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.AssignRoles;

public sealed class AssignRolesCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    UserDtoAssembler userDtoAssembler)
    : IRequestHandler<AssignRolesCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("User.NotFound", "User not found."));

        var requestedIds = request.RoleIds.Distinct().ToHashSet();
        var currentIds = user.RoleIds.ToHashSet();

        foreach (var idToAssign in requestedIds.Except(currentIds))
        {
            user.AssignRole(idToAssign);
        }

        foreach (var idToRevoke in currentIds.Except(requestedIds))
        {
            user.RevokeRole(idToRevoke);
        }

        userRepository.Update(user);
        auditLogService.Log("User", user.Id.ToString(), "RolesChanged", $"Updated role assignments for '{user.Email.Value}' — now has {user.RoleIds.Count} role(s).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await userDtoAssembler.ToDtoAsync(user, cancellationToken);
        return Result.Success(dto);
    }
}
