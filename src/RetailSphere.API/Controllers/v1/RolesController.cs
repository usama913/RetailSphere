using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Roles.CreateRole;
using RetailSphere.Application.Features.Roles.DeleteRole;
using RetailSphere.Application.Features.Roles.GetPermissions;
using RetailSphere.Application.Features.Roles.GetRoles;
using RetailSphere.Application.Features.Roles.UpdateRole;
using RetailSphere.Contracts.Admin;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
[Authorize]
public sealed class RolesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "admin.roles.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("permissions")]
    [Authorize(Policy = "admin.roles.view")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPermissionsQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "admin.roles.create")]
    public async Task<IActionResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateRoleCommand(request.Name, request.Description, request.PermissionIds), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "admin.roles.edit")]
    public async Task<IActionResult> Update(long id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateRoleCommand(id, request.Name, request.Description, request.PermissionIds), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "admin.roles.delete")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteRoleCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
