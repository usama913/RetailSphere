using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.PurchaseOrders.AddPurchaseOrderLine;
using RetailSphere.Application.Features.PurchaseOrders.CancelPurchaseOrder;
using RetailSphere.Application.Features.PurchaseOrders.CreatePurchaseOrder;
using RetailSphere.Application.Features.PurchaseOrders.DeletePurchaseOrder;
using RetailSphere.Application.Features.PurchaseOrders.GetPurchaseOrderById;
using RetailSphere.Application.Features.PurchaseOrders.GetPurchaseOrders;
using RetailSphere.Application.Features.PurchaseOrders.ReceivePurchaseOrderLine;
using RetailSphere.Application.Features.PurchaseOrders.RemovePurchaseOrderLine;
using RetailSphere.Application.Features.PurchaseOrders.SubmitPurchaseOrder;
using RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrder;
using RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrderLine;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/purchase-orders")]
[Authorize]
public sealed class PurchaseOrdersController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "purchasing.orders.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] long? supplierId,
        [FromQuery] long? branchId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetPurchaseOrdersQuery(effectivePage, effectivePageSize, search, supplierId, branchId, status),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "purchasing.orders.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "purchasing.orders.create")]
    public async Task<IActionResult> Create(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePurchaseOrderCommand(request.SupplierId, request.BranchId, request.OrderDate, request.ExpectedDeliveryDate, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> Update(long id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePurchaseOrderCommand(id, request.SupplierId, request.BranchId, request.OrderDate, request.ExpectedDeliveryDate, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeletePurchaseOrderCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/lines")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> AddLine(long id, AddPurchaseOrderLineRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AddPurchaseOrderLineCommand(id, request.ProductId, request.ProductVariantId, request.QuantityOrdered, request.UnitCost),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}/lines/{lineId:long}")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> UpdateLine(long id, long lineId, UpdatePurchaseOrderLineRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePurchaseOrderLineCommand(id, lineId, request.QuantityOrdered, request.UnitCost),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}/lines/{lineId:long}")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> RemoveLine(long id, long lineId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemovePurchaseOrderLineCommand(id, lineId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/submit")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> Submit(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SubmitPurchaseOrderCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/lines/{lineId:long}/receive")]
    [Authorize(Policy = "purchasing.orders.receive")]
    public async Task<IActionResult> ReceiveLine(long id, long lineId, ReceivePurchaseOrderLineRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReceivePurchaseOrderLineCommand(id, lineId, request.Quantity), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/cancel")]
    [Authorize(Policy = "purchasing.orders.edit")]
    public async Task<IActionResult> Cancel(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CancelPurchaseOrderCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
