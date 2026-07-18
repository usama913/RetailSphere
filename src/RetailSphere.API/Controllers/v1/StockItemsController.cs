using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Inventory.AdjustStock;
using RetailSphere.Application.Features.Inventory.GetStockItemById;
using RetailSphere.Application.Features.Inventory.GetStockItems;
using RetailSphere.Application.Features.Inventory.TransferStock;
using RetailSphere.Contracts.Inventory;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stock-items")]
[Authorize]
public sealed class StockItemsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "inventory.stock.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? branchId,
        [FromQuery] long? productId,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(new GetStockItemsQuery(effectivePage, effectivePageSize, branchId, productId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "inventory.stock.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetStockItemByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("adjust")]
    [Authorize(Policy = "inventory.adjustment.create")]
    public async Task<IActionResult> Adjust(AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AdjustStockCommand(request.ProductVariantId, request.BranchId, request.QuantityDelta, request.Reason),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("transfer")]
    [Authorize(Policy = "inventory.adjustment.create")]
    public async Task<IActionResult> Transfer(TransferStockRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new TransferStockCommand(request.ProductVariantId, request.FromBranchId, request.ToBranchId, request.Quantity, request.Reason),
            cancellationToken);
        return HandleResult(result);
    }
}
