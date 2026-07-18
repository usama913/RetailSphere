using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;

namespace RetailSphere.Application.Features.Inventory;

internal static class StockItemMappings
{
    public static StockItemDto ToDto(StockItem stockItem, string? sku, long? productId, string? productName, string? branchName) => new()
    {
        Id = stockItem.Id,
        ProductVariantId = stockItem.ProductVariantId,
        Sku = sku,
        ProductId = productId,
        ProductName = productName,
        BranchId = stockItem.BranchId,
        BranchName = branchName,
        QuantityOnHand = stockItem.QuantityOnHand,
        Adjustments = stockItem.Adjustments
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(ToDto)
            .ToList(),
    };

    public static StockAdjustmentDto ToDto(StockAdjustment adjustment) => new()
    {
        Id = adjustment.Id,
        QuantityDelta = adjustment.QuantityDelta,
        Reason = adjustment.Reason,
        Source = adjustment.Source,
        CreatedAtUtc = adjustment.CreatedAtUtc,
    };
}
