using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Brands.ActivateBrand;
using RetailSphere.Application.Features.Brands.CreateBrand;
using RetailSphere.Application.Features.Brands.DeactivateBrand;
using RetailSphere.Application.Features.Brands.DeleteBrand;
using RetailSphere.Application.Features.Brands.GetBrands;
using RetailSphere.Application.Features.Brands.UpdateBrand;
using RetailSphere.Contracts.Catalog;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/brands")]
[Authorize]
public sealed class BrandsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "catalog.brands.view")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBrandsQuery(includeInactive), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "catalog.brands.edit")]
    public async Task<IActionResult> Create(CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateBrandCommand(request.Name, request.Description), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "catalog.brands.edit")]
    public async Task<IActionResult> Update(long id, UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateBrandCommand(id, request.Name, request.Description), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "catalog.brands.edit")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateBrandCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "catalog.brands.edit")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateBrandCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "catalog.brands.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteBrandCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
