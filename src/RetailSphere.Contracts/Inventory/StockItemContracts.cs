namespace RetailSphere.Contracts.Inventory;

public sealed class StockAdjustmentDto
{
    public required long Id { get; init; }

    public required decimal QuantityDelta { get; init; }

    public required string Reason { get; init; }

    public required string Source { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}

public sealed class StockItemDto
{
    public required long Id { get; init; }

    public required long ProductVariantId { get; init; }

    public string? Sku { get; init; }

    public long? ProductId { get; init; }

    public string? ProductName { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public required decimal QuantityOnHand { get; init; }

    /// <summary>Purchase/landed cost of the variant, resolved from the current Product/ProductVariant — used to compute StockValue.</summary>
    public decimal? CostPrice { get; init; }

    /// <summary>QuantityOnHand * CostPrice (0 when CostPrice isn't set) — the money value of stock on hand at this branch.</summary>
    public required decimal StockValue { get; init; }

    /// <summary>The variant's low-stock threshold (null = no threshold set), resolved from the current Product/ProductVariant.</summary>
    public decimal? ReorderPoint { get; init; }

    /// <summary>True when ReorderPoint is set and QuantityOnHand has dropped to or below it.</summary>
    public required bool IsLowStock { get; init; }

    /// <summary>The variant's expiry/best-before date (null = not tracked for expiry), resolved from the current ProductVariant.</summary>
    public DateTime? ExpiryDate { get; init; }

    public required IReadOnlyList<StockAdjustmentDto> Adjustments { get; init; }
}

public sealed class AdjustStockRequest
{
    public required long ProductVariantId { get; init; }

    public required long BranchId { get; init; }

    public required decimal QuantityDelta { get; init; }

    public required string Reason { get; init; }
}

/// <summary>
/// A one-step move of stock between two branches — decrements FromBranchId and
/// increments ToBranchId atomically. This is deliberately lighter-weight than the
/// full Stock Transfer workflow (create → submit → receive), for the common case of
/// just correcting/moving a quantity right now without a paper trail across statuses.
/// </summary>
public sealed class TransferStockRequest
{
    public required long ProductVariantId { get; init; }

    public required long FromBranchId { get; init; }

    public required long ToBranchId { get; init; }

    public required decimal Quantity { get; init; }

    public required string Reason { get; init; }
}
