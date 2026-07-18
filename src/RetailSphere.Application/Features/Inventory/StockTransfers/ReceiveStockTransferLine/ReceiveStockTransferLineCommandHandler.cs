using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.ReceiveStockTransferLine;

/// <summary>
/// Records received quantity against a line AND moves branch stock — this is the
/// Inventory integration point for transfers, the same role
/// ReceivePurchaseOrderLineCommandHandler plays for purchase orders. Unlike a PO
/// (which only ever adds stock, from an external supplier), a transfer has two legs:
/// stock is decremented at FromBranchId and incremented at ToBranchId for the same
/// ProductVariantId, by the same quantity. Decrementing first means an
/// insufficient-stock-at-source failure (StockItem.AdjustQuantity rejects negative
/// results) naturally aborts the whole receive before anything is credited at the
/// destination.
/// </summary>
public sealed class ReceiveStockTransferLineCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IStockItemRepository stockItemRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<ReceiveStockTransferLineCommand, Result<StockTransferDto>>
{
    public async Task<Result<StockTransferDto>> Handle(ReceiveStockTransferLineCommand request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.StockTransferId, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        var receiveResult = stockTransfer.ReceiveLine(request.LineId, request.Quantity);
        if (receiveResult.IsFailure)
            return Result.Failure<StockTransferDto>(receiveResult.Error);

        var line = stockTransfer.Lines.First(l => l.Id == request.LineId);

        var fromBranch = await branchRepository.GetByIdAsync(stockTransfer.FromBranchId, cancellationToken);
        var toBranch = await branchRepository.GetByIdAsync(stockTransfer.ToBranchId, cancellationToken);

        var sourceItem = await stockItemRepository.GetByVariantAndBranchAsync(line.ProductVariantId, stockTransfer.FromBranchId, cancellationToken);
        if (sourceItem is null)
        {
            var createSourceResult = StockItem.Create(line.ProductVariantId, stockTransfer.FromBranchId);
            if (createSourceResult.IsFailure)
                return Result.Failure<StockTransferDto>(createSourceResult.Error);

            sourceItem = createSourceResult.Value;
            stockItemRepository.Add(sourceItem);

            // Two-phase save: the StockAdjustment logged below needs this StockItem's
            // real (post-insert) Id — same reason every other create-then-log handler
            // in this codebase saves twice (see AdjustStockCommandHandler).
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var deductResult = sourceItem.AdjustQuantity(
            -request.Quantity,
            $"Transferred out on '{stockTransfer.TransferNumber}' to branch '{toBranch?.Name ?? stockTransfer.ToBranchId.ToString()}'.",
            "StockTransferOut");
        if (deductResult.IsFailure)
            return Result.Failure<StockTransferDto>(deductResult.Error);

        var destinationItem = await stockItemRepository.GetByVariantAndBranchAsync(line.ProductVariantId, stockTransfer.ToBranchId, cancellationToken);
        if (destinationItem is null)
        {
            var createDestinationResult = StockItem.Create(line.ProductVariantId, stockTransfer.ToBranchId);
            if (createDestinationResult.IsFailure)
                return Result.Failure<StockTransferDto>(createDestinationResult.Error);

            destinationItem = createDestinationResult.Value;
            stockItemRepository.Add(destinationItem);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var addResult = destinationItem.AdjustQuantity(
            request.Quantity,
            $"Transferred in on '{stockTransfer.TransferNumber}' from branch '{fromBranch?.Name ?? stockTransfer.FromBranchId.ToString()}'.",
            "StockTransferIn");
        if (addResult.IsFailure)
            return Result.Failure<StockTransferDto>(addResult.Error);

        stockItemRepository.Update(sourceItem);
        stockItemRepository.Update(destinationItem);
        stockTransferRepository.Update(stockTransfer);
        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "LineReceived", $"Received {request.Quantity} unit(s) on stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }
}
