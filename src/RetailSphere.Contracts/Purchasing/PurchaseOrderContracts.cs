namespace RetailSphere.Contracts.Purchasing;

public sealed class PurchaseOrderLineDto
{
    public required long Id { get; init; }

    public required long ProductId { get; init; }

    public required long ProductVariantId { get; init; }

    public required string SkuSnapshot { get; init; }

    public required string DescriptionSnapshot { get; init; }

    public required decimal QuantityOrdered { get; init; }

    public required decimal QuantityReceived { get; init; }

    public required decimal UnitCost { get; init; }

    public required decimal LineTotal { get; init; }

    public required decimal RemainingQuantity { get; init; }

    public required bool IsFullyReceived { get; init; }
}

public sealed class PurchaseOrderDto
{
    public required long Id { get; init; }

    public required string PoNumber { get; init; }

    public required long SupplierId { get; init; }

    public string? SupplierName { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public required string Status { get; init; }

    public required DateTime OrderDate { get; init; }

    public DateTime? ExpectedDeliveryDate { get; init; }

    public string? Notes { get; init; }

    public required decimal TotalAmount { get; init; }

    public required IReadOnlyList<PurchaseOrderLineDto> Lines { get; init; }
}

public sealed class CreatePurchaseOrderRequest
{
    public required long SupplierId { get; init; }

    public required long BranchId { get; init; }

    public required DateTime OrderDate { get; init; }

    public DateTime? ExpectedDeliveryDate { get; init; }

    public string? Notes { get; init; }
}

public sealed class UpdatePurchaseOrderRequest
{
    public required long SupplierId { get; init; }

    public required long BranchId { get; init; }

    public required DateTime OrderDate { get; init; }

    public DateTime? ExpectedDeliveryDate { get; init; }

    public string? Notes { get; init; }
}

public sealed class AddPurchaseOrderLineRequest
{
    public required long ProductId { get; init; }

    public required long ProductVariantId { get; init; }

    public required decimal QuantityOrdered { get; init; }

    public required decimal UnitCost { get; init; }
}

public sealed class UpdatePurchaseOrderLineRequest
{
    public required decimal QuantityOrdered { get; init; }

    public required decimal UnitCost { get; init; }
}

public sealed class ReceivePurchaseOrderLineRequest
{
    public required decimal Quantity { get; init; }
}
