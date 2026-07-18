using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Inventory.StockTransfers.AddStockTransferLine;
using RetailSphere.Application.Features.Inventory.StockTransfers.CancelStockTransfer;
using RetailSphere.Application.Features.Inventory.StockTransfers.CreateStockTransfer;
using RetailSphere.Application.Features.Inventory.StockTransfers.DeleteStockTransfer;
using RetailSphere.Application.Features.Inventory.StockTransfers.GetStockTransferById;
using RetailSphere.Application.Features.Inventory.StockTransfers.GetStockTransfers;
using RetailSphere.Application.Features.Inventory.StockTransfers.ReceiveStockTransferLine;
using RetailSphere.Application.Features.Inventory.StockTransfers.RemoveStockTransferLine;
using RetailSphere.Application.Features.Inventory.StockTransfers.SubmitStockTransfer;
using RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransfer;
using RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransferLine;
using RetailSphere.Contracts.Inventory;

namespace RetailSphere.API.Controllers.v1;

/// <summary>
/// Only two permission codes exist for transfers (unlike Purchasing's separate
/// view/create/edit/receive set): "inventory.transfer.view" gates everything about
/// building and viewing a draft transfer request, and "inventory.transfer.approve"
/// gates the lifecycle transitions that actually commit it — submitting it,
/// receiving stock against it, and cancelling it.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stock-transfers")]
[Authorize]
public sealed class StockTransfersController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] long? fromBranchId,
        [FromQuery] long? toBranchId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetStockTransfersQuery(effectivePage, effectivePageSize, search, fromBranchId, toBranchId, status),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetStockTransferByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> Create(CreateStockTransferRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateStockTransferCommand(request.FromBranchId, request.ToBranchId, request.TransferDate, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> Update(long id, UpdateStockTransferRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateStockTransferCommand(id, request.FromBranchId, request.ToBranchId, request.TransferDate, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteStockTransferCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/lines")]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> AddLine(long id, AddStockTransferLineRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AddStockTransferLineCommand(id, request.ProductId, request.ProductVariantId, request.QuantityRequested),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}/lines/{lineId:long}")]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> UpdateLine(long id, long lineId, UpdateStockTransferLineRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateStockTransferLineCommand(id, lineId, request.QuantityRequested),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}/lines/{lineId:long}")]
    [Authorize(Policy = "inventory.transfer.view")]
    public async Task<IActionResult> RemoveLine(long id, long lineId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveStockTransferLineCommand(id, lineId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/submit")]
    [Authorize(Policy = "inventory.transfer.approve")]
    public async Task<IActionResult> Submit(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SubmitStockTransferCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/lines/{lineId:long}/receive")]
    [Authorize(Policy = "inventory.transfer.approve")]
    public async Task<IActionResult> ReceiveLine(long id, long lineId, ReceiveStockTransferLineRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReceiveStockTransferLineCommand(id, lineId, request.Quantity), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/cancel")]
    [Authorize(Policy = "inventory.transfer.approve")]
    public async Task<IActionResult> Cancel(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CancelStockTransferCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
