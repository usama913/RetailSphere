using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Users.ActivateUser;
using RetailSphere.Application.Features.Users.AssignRoles;
using RetailSphere.Application.Features.Users.CreateUser;
using RetailSphere.Application.Features.Users.DeactivateUser;
using RetailSphere.Application.Features.Users.GetUserById;
using RetailSphere.Application.Features.Users.GetUsers;
using RetailSphere.Application.Features.Users.ResetPassword;
using RetailSphere.Application.Features.Users.UpdateUser;
using RetailSphere.Contracts.Admin;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public sealed class UsersController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "admin.users.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] long? branchId,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(new GetUsersQuery(effectivePage, effectivePageSize, search, branchId, isActive), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "admin.users.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "admin.users.create")]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateUserCommand(request.Email, request.Password, request.FirstName, request.LastName, request.BranchId, request.RoleIds),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "admin.users.edit")]
    public async Task<IActionResult> Update(long id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateUserCommand(id, request.FirstName, request.LastName, request.BranchId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}/roles")]
    [Authorize(Policy = "admin.users.edit")]
    public async Task<IActionResult> AssignRoles(long id, AssignRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AssignRolesCommand(id, request.RoleIds), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "admin.users.deactivate")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateUserCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "admin.users.deactivate")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateUserCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/reset-password")]
    [Authorize(Policy = "admin.users.edit")]
    public async Task<IActionResult> ResetPassword(long id, ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ResetPasswordCommand(id, request.NewPassword), cancellationToken);
        return HandleResult(result);
    }
}
