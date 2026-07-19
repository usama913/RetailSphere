namespace RetailSphere.Contracts.Purchasing;

public sealed class PurchaseInvoiceDto
{
    public required long Id { get; init; }

    public required long SupplierId { get; init; }

    public string? SupplierName { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public long? PurchaseOrderId { get; init; }

    public string? PurchaseOrderNumber { get; init; }

    public required string SupplierInvoiceNumber { get; init; }

    public required DateTime InvoiceDate { get; init; }

    public required DateTime DueDate { get; init; }

    public required string PaymentTerms { get; init; }

    public required decimal SubtotalAmount { get; init; }

    public required decimal DiscountAmount { get; init; }

    public required decimal TaxAmount { get; init; }

    public required decimal TotalAmount { get; init; }

    public required decimal AmountPaid { get; init; }

    public required decimal OutstandingBalance { get; init; }

    public required string PaymentStatus { get; init; }

    public string? Notes { get; init; }
}

public sealed class CreatePurchaseInvoiceRequest
{
    public required long SupplierId { get; init; }

    public required long BranchId { get; init; }

    public long? PurchaseOrderId { get; init; }

    public required string SupplierInvoiceNumber { get; init; }

    public required DateTime InvoiceDate { get; init; }

    public required DateTime DueDate { get; init; }

    public string? PaymentTerms { get; init; }

    public required decimal SubtotalAmount { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public string? Notes { get; init; }
}

public sealed class UpdatePurchaseInvoiceRequest
{
    public required string SupplierInvoiceNumber { get; init; }

    public required DateTime InvoiceDate { get; init; }

    public required DateTime DueDate { get; init; }

    public string? PaymentTerms { get; init; }

    public required decimal SubtotalAmount { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public string? Notes { get; init; }
}
