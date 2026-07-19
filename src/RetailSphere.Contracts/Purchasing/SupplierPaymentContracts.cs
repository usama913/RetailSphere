namespace RetailSphere.Contracts.Purchasing;

public sealed class SupplierPaymentDto
{
    public required long Id { get; init; }

    public required long SupplierId { get; init; }

    public string? SupplierName { get; init; }

    public required long PurchaseInvoiceId { get; init; }

    public string? PurchaseInvoiceSupplierInvoiceNumber { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public required DateTime PaymentDate { get; init; }

    public required decimal Amount { get; init; }

    public required string PaymentMethod { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Notes { get; init; }

    public required bool IsReversed { get; init; }

    public string? ReversalReason { get; init; }

    public DateTime? ReversedAtUtc { get; init; }

    public string? ReversedByUserName { get; init; }
}

public sealed class RecordSupplierPaymentRequest
{
    public required long SupplierId { get; init; }

    public required long PurchaseInvoiceId { get; init; }

    public required long BranchId { get; init; }

    public required DateTime PaymentDate { get; init; }

    public required decimal Amount { get; init; }

    public string? PaymentMethod { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Notes { get; init; }
}

public sealed class UpdateSupplierPaymentRequest
{
    public required DateTime PaymentDate { get; init; }

    public required decimal Amount { get; init; }

    public string? PaymentMethod { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Notes { get; init; }
}

public sealed class ReverseSupplierPaymentRequest
{
    public required string Reason { get; init; }
}
