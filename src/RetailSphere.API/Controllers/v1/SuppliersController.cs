using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Suppliers.ActivateSupplier;
using RetailSphere.Application.Features.Suppliers.CreateSupplier;
using RetailSphere.Application.Features.Suppliers.DeactivateSupplier;
using RetailSphere.Application.Features.Suppliers.DeleteSupplier;
using RetailSphere.Application.Features.Suppliers.GetSuppliers;
using RetailSphere.Application.Features.Suppliers.UpdateSupplier;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/suppliers")]
[Authorize]
public sealed class SuppliersController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "purchasing.suppliers.view")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSuppliersQuery(includeInactive), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "purchasing.suppliers.edit")]
    public async Task<IActionResult> Create(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateSupplierCommand(request.Name, request.ContactPerson, request.Email, request.Phone, request.Address, request.TaxNumber),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "purchasing.suppliers.edit")]
    public async Task<IActionResult> Update(long id, UpdateSupplierRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateSupplierCommand(id, request.Name, request.ContactPerson, request.Email, request.Phone, request.Address, request.TaxNumber),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "purchasing.suppliers.edit")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateSupplierCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "purchasing.suppliers.edit")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateSupplierCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "purchasing.suppliers.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteSupplierCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
