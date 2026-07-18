using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Units.ActivateUnit;
using RetailSphere.Application.Features.Units.CreateUnit;
using RetailSphere.Application.Features.Units.DeactivateUnit;
using RetailSphere.Application.Features.Units.DeleteUnit;
using RetailSphere.Application.Features.Units.GetUnits;
using RetailSphere.Application.Features.Units.UpdateUnit;
using RetailSphere.Contracts.Catalog;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/units")]
[Authorize]
public sealed class UnitsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "catalog.units.view")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUnitsQuery(includeInactive), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "catalog.units.edit")]
    public async Task<IActionResult> Create(CreateUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateUnitCommand(request.Name, request.ShortCode, request.AllowDecimal), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "catalog.units.edit")]
    public async Task<IActionResult> Update(long id, UpdateUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateUnitCommand(id, request.Name, request.ShortCode, request.AllowDecimal), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "catalog.units.edit")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateUnitCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "catalog.units.edit")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateUnitCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "catalog.units.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteUnitCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
