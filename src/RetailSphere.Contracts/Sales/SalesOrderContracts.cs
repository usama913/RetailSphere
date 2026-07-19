namespace RetailSphere.Contracts.Sales;

public sealed class SalesOrderLineDto
{
    public required long Id { get; init; }

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

    public required decimal LineTotal { get; init; }

    public required decimal CostPriceSnapshot { get; init; }

    public required decimal QuantityReturned { get; init; }
}

public sealed class SalesOrderDto
{
    public required long Id { get; init; }

    public required string OrderNumber { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public long? CustomerId { get; init; }

    public string? CustomerName { get; init; }

    public long? CashierUserId { get; init; }

    public string? CashierName { get; init; }

    public required string Status { get; init; }

    public required DateTime OrderDate { get; init; }

    public DateTime? DueDate { get; init; }

    public string? PaymentTerms { get; init; }

    public required string PaymentMethod { get; init; }

    public required decimal SubtotalAmount { get; init; }

    public required decimal TaxAmount { get; init; }

    public required decimal OrderDiscountAmount { get; init; }

    public required decimal TotalAmount { get; init; }

    public required decimal AmountPaid { get; init; }

    public required decimal OutstandingBalance { get; init; }

    public required string PaymentStatus { get; init; }

    public required decimal ChangeDue { get; init; }

    public string? Notes { get; init; }

    public string? CancellationReason { get; init; }

    public required IReadOnlyList<SalesOrderLineDto> Lines { get; init; }
}

public sealed class CreateSalesOrderLineRequest
{
    public required long ProductId { get; init; }

    public required long ProductVariantId { get; init; }

    public required decimal Quantity { get; init; }

    public decimal DiscountAmount { get; init; }
}

/// <summary>The POS checkout payload — server resolves each line's current price/tax from the catalog rather than trusting client-supplied amounts (see CreateSalesOrderCommandHandler).</summary>
public sealed class CreateSalesOrderRequest
{
    public required long BranchId { get; init; }

    public long? CustomerId { get; init; }

    public string? PaymentMethod { get; init; }

    public decimal OrderDiscountAmount { get; init; }

    public required decimal AmountPaid { get; init; }

    public string? Notes { get; init; }

    public string? PaymentTerms { get; init; }

    public DateTime? DueDate { get; init; }

    /// <summary>Set true to resubmit a checkout the server previously rejected with SalesOrder.CreditLimitExceeded, after the cashier/manager confirmed the override.</summary>
    public bool OverrideCreditLimit { get; init; }

    public required IReadOnlyList<CreateSalesOrderLineRequest> Lines { get; init; }
}

public sealed class CancelSalesOrderRequest
{
    public required string Reason { get; init; }
}
