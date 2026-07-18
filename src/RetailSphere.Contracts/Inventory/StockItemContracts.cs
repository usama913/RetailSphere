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
