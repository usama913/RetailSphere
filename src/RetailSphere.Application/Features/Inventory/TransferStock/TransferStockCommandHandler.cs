using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.TransferStock;

/// <summary>
/// One-step branch-to-branch stock move: decrements the StockItem at FromBranchId and
/// increments the StockItem at ToBranchId for the same ProductVariantId, in a single
/// action — no separate Draft/Submit/Receive lifecycle (that's what the Stock
/// Transfers module under Inventory.StockTransfers is for, when a paper trail across
/// statuses matters). This exists because AdjustStock only ever touches one branch at
/// a time; a user trying to "move" stock by doing two separate AdjustStock calls can
/// easily do just one side of it and leave the source branch's quantity unchanged —
/// exactly the bug this command prevents by doing both sides atomically.
/// Deducting first means insufficient stock at the source (StockItem.AdjustQuantity
/// rejects negative results) naturally aborts before anything is credited at the destination.
/// </summary>
public sealed class TransferStockCommandHandler(
    IStockItemRepository stockItemRepository,
    IProductRepository productRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<TransferStockCommand, Result>
{
    public async Task<Result> Handle(TransferStockCommand request, CancellationToken cancellationToken)
    {
        var owningProducts = await productRepository.GetByVariantIdsAsync([request.ProductVariantId], cancellationToken);
        if (owningProducts.Count == 0 || owningProducts[0].Variants.All(v => v.Id != request.ProductVariantId))
            return Result.Failure(Error.NotFound("ProductVariant.NotFound", "Product variant not found."));

        var fromBranch = await branchRepository.GetByIdAsync(request.FromBranchId, cancellationToken);
        if (fromBranch is null)
            return Result.Failure(Error.NotFound("Branch.NotFound", "Source branch not found."));

        var toBranch = await branchRepository.GetByIdAsync(request.ToBranchId, cancellationToken);
        if (toBranch is null)
            return Result.Failure(Error.NotFound("Branch.NotFound", "Destination branch not found."));

        var sourceItem = await stockItemRepository.GetByVariantAndBranchAsync(request.ProductVariantId, request.FromBranchId, cancellationToken);
        if (sourceItem is null)
        {
            var createSourceResult = StockItem.Create(request.ProductVariantId, request.FromBranchId);
            if (createSourceResult.IsFailure)
                return Result.Failure(createSourceResult.Error);

            sourceItem = createSourceResult.Value;
            stockItemRepository.Add(sourceItem);

            // Two-phase save: the StockAdjustment logged just below needs this
            // StockItem's real (post-insert) Id — same reason every other
            // create-then-log handler in this codebase saves twice.
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var deductResult = sourceItem.AdjustQuantity(
            -request.Quantity,
            $"Transferred to branch '{toBranch.Name}': {request.Reason}",
            "StockTransferOut");
        if (deductResult.IsFailure)
            return Result.Failure(deductResult.Error);

        var destinationItem = await stockItemRepository.GetByVariantAndBranchAsync(request.ProductVariantId, request.ToBranchId, cancellationToken);
        if (destinationItem is null)
        {
            var createDestinationResult = StockItem.Create(request.ProductVariantId, request.ToBranchId);
            if (createDestinationResult.IsFailure)
                return Result.Failure(createDestinationResult.Error);

            destinationItem = createDestinationResult.Value;
            stockItemRepository.Add(destinationItem);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var addResult = destinationItem.AdjustQuantity(
            request.Quantity,
            $"Transferred from branch '{fromBranch.Name}': {request.Reason}",
            "StockTransferIn");
        if (addResult.IsFailure)
            return Result.Failure(addResult.Error);

        stockItemRepository.Update(sourceItem);
        stockItemRepository.Update(destinationItem);
        auditLogService.Log(
            "StockItem",
            sourceItem.Id.ToString(),
            "Transferred",
            $"Transferred {request.Quantity} unit(s) of variant {request.ProductVariantId} from '{fromBranch.Name}' to '{toBranch.Name}': {request.Reason}");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
