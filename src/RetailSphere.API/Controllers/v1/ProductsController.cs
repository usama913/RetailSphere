using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Products.ActivateProduct;
using RetailSphere.Application.Features.Products.AddProductImage;
using RetailSphere.Application.Features.Products.AddVariant;
using RetailSphere.Application.Features.Products.CreateProduct;
using RetailSphere.Application.Features.Products.DeactivateProduct;
using RetailSphere.Application.Features.Products.DeleteProduct;
using RetailSphere.Application.Features.Products.GetProductById;
using RetailSphere.Application.Features.Products.GetProducts;
using RetailSphere.Application.Features.Products.RemoveProductImage;
using RetailSphere.Application.Features.Products.RemoveVariant;
using RetailSphere.Application.Features.Products.SetVariantActive;
using RetailSphere.Application.Features.Products.UpdateProduct;
using RetailSphere.Application.Features.Products.UpdateVariant;
using RetailSphere.Contracts.Catalog;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
[Authorize]
public sealed class ProductsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "catalog.products.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] long? categoryId,
        [FromQuery] long? brandId,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetProductsQuery(effectivePage, effectivePageSize, search, categoryId, brandId, isActive),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "catalog.products.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateProductCommand(
                request.Name,
                request.Description,
                request.CategoryId,
                request.BrandId,
                request.UnitId,
                request.ManageStock,
                request.NotForSelling),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Update(long id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProductCommand(
                id,
                request.Name,
                request.Description,
                request.CategoryId,
                request.BrandId,
                request.UnitId,
                request.ManageStock,
                request.NotForSelling),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateProductCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateProductCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/variants")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> AddVariant(long id, AddVariantRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AddVariantCommand(
                id,
                request.Sku,
                request.Barcode,
                request.BarcodeType,
                request.Price,
                request.CompareAtPrice,
                request.CostPrice,
                request.TaxRate,
                request.TaxType,
                request.Weight,
                request.Length,
                request.Width,
                request.Height,
                request.ReorderPoint,
                request.AttributeValueIds,
                request.ExpiryDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}/variants/{variantId:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> UpdateVariant(long id, long variantId, UpdateVariantRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateVariantCommand(
                id,
                variantId,
                request.Barcode,
                request.BarcodeType,
                request.Price,
                request.CompareAtPrice,
                request.CostPrice,
                request.TaxRate,
                request.TaxType,
                request.Weight,
                request.Length,
                request.Width,
                request.Height,
                request.ReorderPoint,
                request.AttributeValueIds,
                request.ExpiryDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}/variants/{variantId:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> RemoveVariant(long id, long variantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveVariantCommand(id, variantId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/variants/{variantId:long}/activate")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> ActivateVariant(long id, long variantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetVariantActiveCommand(id, variantId, true), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/variants/{variantId:long}/deactivate")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> DeactivateVariant(long id, long variantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetVariantActiveCommand(id, variantId, false), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/images")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> AddImage(long id, AddProductImageRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AddProductImageCommand(id, request.Url), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}/images/{imageId:long}")]
    [Authorize(Policy = "catalog.products.edit")]
    public async Task<IActionResult> RemoveImage(long id, long imageId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveProductImageCommand(id, imageId), cancellationToken);
        return HandleResult(result);
    }
}
