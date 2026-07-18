namespace RetailSphere.Contracts.Inventory;

public sealed class StockTransferLineDto
{
    public required long Id { get; init; }

    public required long ProductId { get; init; }

    public required long ProductVariantId { get; init; }

    public required string SkuSnapshot { get; init; }

    public required string DescriptionSnapshot { get; init; }

    public required decimal QuantityRequested { get; init; }

    public required decimal QuantityReceived { get; init; }

    public required decimal RemainingQuantity { get; init; }

    public required bool IsFullyReceived { get; init; }
}

public sealed class StockTransferDto
{
    public required long Id { get; init; }

    public required string TransferNumber { get; init; }

    public required long FromBranchId { get; init; }

    public string? FromBranchName { get; init; }

    public required long ToBranchId { get; init; }

    public string? ToBranchName { get; init; }

    public required string Status { get; init; }

    public required DateTime TransferDate { get; init; }

    public string? Notes { get; init; }

    public required decimal TotalQuantityRequested { get; init; }

    public required IReadOnlyList<StockTransferLineDto> Lines { get; init; }
}

public sealed class CreateStockTransferRequest
{
    public required long FromBranchId { get; init; }

    public required long ToBranchId { get; init; }

    public required DateTime TransferDate { get; init; }

    public string? Notes { get; init; }
}

public sealed class UpdateStockTransferRequest
{
    public required long FromBranchId { get; init; }

    public required long ToBranchId { get; init; }

    public required DateTime TransferDate { get; init; }

    public string? Notes { get; init; }
}

public sealed class AddStockTransferLineRequest
{
    public required long ProductId { get; init; }

    public required long ProductVariantId { get; init; }

    public required decimal QuantityRequested { get; init; }
}

public sealed class UpdateStockTransferLineRequest
{
    public required decimal QuantityRequested { get; init; }
}

public sealed class ReceiveStockTransferLineRequest
{
    public required decimal Quantity { get; init; }
}
