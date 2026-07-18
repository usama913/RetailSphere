using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.ReceivePurchaseOrderLine;

/// <summary>
/// Records received quantity against a line AND increments branch stock — this is
/// the Inventory integration point flagged (but deliberately not implemented) when
/// the Purchasing module was first built, now that StockItem exists. Stock is
/// incremented at the PurchaseOrder's own BranchId, for the line's ProductVariantId.
/// </summary>
public sealed class ReceivePurchaseOrderLineCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IStockItemRepository stockItemRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<ReceivePurchaseOrderLineCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(ReceivePurchaseOrderLineCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var receiveResult = purchaseOrder.ReceiveLine(request.LineId, request.Quantity);
        if (receiveResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(receiveResult.Error);

        var line = purchaseOrder.Lines.First(l => l.Id == request.LineId);

        var stockItem = await stockItemRepository.GetByVariantAndBranchAsync(line.ProductVariantId, purchaseOrder.BranchId, cancellationToken);
        if (stockItem is null)
        {
            var createResult = StockItem.Create(line.ProductVariantId, purchaseOrder.BranchId);
            if (createResult.IsFailure)
                return Result.Failure<PurchaseOrderDto>(createResult.Error);

            stockItem = createResult.Value;
            stockItemRepository.Add(stockItem);

            // Two-phase save: the stock adjustment logged just below needs this
            // StockItem's real (post-insert) Id — same reason every other
            // create-then-log handler in this codebase saves twice.
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var adjustResult = stockItem.AdjustQuantity(
            request.Quantity,
            $"Received on purchase order '{purchaseOrder.PoNumber}'.",
            "PurchaseOrderReceipt");
        if (adjustResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(adjustResult.Error);

        stockItemRepository.Update(stockItem);
        purchaseOrderRepository.Update(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "LineReceived", $"Received {request.Quantity} unit(s) on purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
