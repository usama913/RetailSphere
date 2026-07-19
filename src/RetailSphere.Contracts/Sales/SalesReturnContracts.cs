namespace RetailSphere.Contracts.Sales;

public sealed class SalesReturnLineDto
{
    public required long Id { get; init; }

    public required long SalesOrderLineId { get; init; }

    public required long ProductId { get; init; }

    public required long ProductVariantId { get; init; }

    public required string SkuSnapshot { get; init; }

    public required string DescriptionSnapshot { get; init; }

    public required decimal Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public required decimal TaxRateSnapshot { get; init; }

    public required string TaxTypeSnapshot { get; init; }

    public required decimal DiscountAmount { get; init; }

    public required decimal TaxAmount { get; init; }

    public required decimal RefundAmount { get; init; }
}

public sealed class SalesReturnDto
{
    public required long Id { get; init; }

    public required string ReturnNumber { get; init; }

    public required long SalesOrderId { get; init; }

    public string? SalesOrderNumber { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public long? CustomerId { get; init; }

    public string? CustomerName { get; init; }

    public long? ProcessedByUserId { get; init; }

    public string? ProcessedByUserName { get; init; }

    public required DateTime ReturnDate { get; init; }

    public string? Reason { get; init; }

    public required decimal RefundAmount { get; init; }

    public required IReadOnlyList<SalesReturnLineDto> Lines { get; init; }
}

public sealed class CreateSalesReturnLineRequest
{
    /// <summary>The original SalesOrderLine being (partially or fully) returned.</summary>
    public required long SalesOrderLineId { get; init; }

    public required decimal Quantity { get; init; }
}

/// <summary>
/// Search the original invoice by SalesOrderId (resolved client-side from the invoice
/// number via GET /sales-orders?search=...), then submit the items/quantities being
/// returned. Price/tax/discount are always resolved server-side from the original
/// SalesOrderLine, never trusted from the client — same rule as CreateSalesOrderRequest.
/// </summary>
public sealed class CreateSalesReturnRequest
{
    public required long SalesOrderId { get; init; }

    public string? Reason { get; init; }

    public required IReadOnlyList<CreateSalesReturnLineRequest> Lines { get; init; }
}
