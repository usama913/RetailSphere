using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Branches.ActivateBranch;
using RetailSphere.Application.Features.Branches.CreateBranch;
using RetailSphere.Application.Features.Branches.DeactivateBranch;
using RetailSphere.Application.Features.Branches.GetBranches;
using RetailSphere.Application.Features.Branches.UpdateBranch;
using RetailSphere.Contracts.Admin;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/branches")]
[Authorize]
public sealed class BranchesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "admin.branches.view")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBranchesQuery(includeInactive), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "admin.branches.create")]
    public async Task<IActionResult> Create(CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateBranchCommand(request.Name, request.Code, request.Address, request.City, request.CurrencyCode),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "admin.branches.edit")]
    public async Task<IActionResult> Update(long id, UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateBranchCommand(id, request.Name, request.Address, request.City, request.CurrencyCode),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "admin.branches.deactivate")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateBranchCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "admin.branches.deactivate")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateBranchCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
