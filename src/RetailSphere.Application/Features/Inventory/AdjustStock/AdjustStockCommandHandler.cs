using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.AdjustStock;

/// <summary>
/// The only way stock quantities change via direct user action (as opposed to
/// PurchaseOrder receiving, which calls the same get-or-create-then-adjust shape
/// from ReceivePurchaseOrderLineCommandHandler with Source = "PurchaseOrderReceipt").
/// A StockItem is created lazily here the first time a variant/branch combination
/// is ever adjusted — variants start with no stock row at all (see Products.razor's
/// variant generator, which deliberately doesn't set an opening quantity).
/// </summary>
public sealed class AdjustStockCommandHandler(
    IStockItemRepository stockItemRepository,
    IProductRepository productRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockItemDtoAssembler stockItemDtoAssembler)
    : IRequestHandler<AdjustStockCommand, Result<StockItemDto>>
{
    public async Task<Result<StockItemDto>> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var owningProducts = await productRepository.GetByVariantIdsAsync([request.ProductVariantId], cancellationToken);
        if (owningProducts.Count == 0 || owningProducts[0].Variants.All(v => v.Id != request.ProductVariantId))
            return Result.Failure<StockItemDto>(Error.NotFound("ProductVariant.NotFound", "Product variant not found."));

        var branch = await branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result.Failure<StockItemDto>(Error.NotFound("Branch.NotFound", "Branch not found."));

        var stockItem = await stockItemRepository.GetByVariantAndBranchAsync(request.ProductVariantId, request.BranchId, cancellationToken);
        if (stockItem is null)
        {
            var createResult = StockItem.Create(request.ProductVariantId, request.BranchId);
            if (createResult.IsFailure)
                return Result.Failure<StockItemDto>(createResult.Error);

            stockItem = createResult.Value;
            stockItemRepository.Add(stockItem);

            // Two-phase save: StockAdjustment.Create needs the parent's real (post-insert)
            // Id, the same reason PurchaseOrder/Product create handlers save once before
            // logging — see CreatePurchaseOrderCommandHandler for the established pattern.
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var adjustResult = stockItem.AdjustQuantity(request.QuantityDelta, request.Reason, "Manual");
        if (adjustResult.IsFailure)
            return Result.Failure<StockItemDto>(adjustResult.Error);

        stockItemRepository.Update(stockItem);
        auditLogService.Log(
            "StockItem",
            stockItem.Id.ToString(),
            "Adjusted",
            $"Adjusted stock by {request.QuantityDelta} for variant {request.ProductVariantId} at branch '{branch.Name}': {request.Reason}");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockItemDtoAssembler.ToDtoAsync(stockItem, cancellationToken);
        return Result.Success(dto);
    }
}
