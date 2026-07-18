using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.ProductAttributes.AddAttributeValue;
using RetailSphere.Application.Features.ProductAttributes.CreateProductAttribute;
using RetailSphere.Application.Features.ProductAttributes.DeleteProductAttribute;
using RetailSphere.Application.Features.ProductAttributes.GetProductAttributes;
using RetailSphere.Application.Features.ProductAttributes.RemoveAttributeValue;
using RetailSphere.Application.Features.ProductAttributes.UpdateProductAttribute;
using RetailSphere.Contracts.Catalog;

namespace RetailSphere.API.Controllers.v1;

// Attributes (Size/Color/Material) are supporting reference data for
// Products — folded into the "catalog.products.*" permissions rather than
// their own codes, and surfaced in the UI from within the Products area
// (there's no separate "Attributes" nav item — see Sidebar.razor).
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/product-attributes")]
[Authorize]
public sealed class ProductAttributesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "catalog.products.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductAttributesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Create(CreateProductAttributeRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateProductAttributeCommand(request.Name), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Update(long id, UpdateProductAttributeRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateProductAttributeCommand(id, request.Name), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProductAttributeCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/values")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> AddValue(long id, AddAttributeValueRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AddAttributeValueCommand(id, request.Value), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}/values/{valueId:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> RemoveValue(long id, long valueId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveAttributeValueCommand(id, valueId), cancellationToken);
        return HandleResult(result);
    }
}
