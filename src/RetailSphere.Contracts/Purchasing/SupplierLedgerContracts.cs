namespace RetailSphere.Contracts.Purchasing;

public sealed class SupplierLedgerEntryDto
{
    public required DateTime Date { get; init; }

    /// <summary>"Invoice" or "Payment".</summary>
    public required string Type { get; init; }

    public string? ReferenceNumber { get; init; }

    public required string Description { get; init; }

    /// <summary>Increases what we owe the supplier (a new invoice).</summary>
    public required decimal DebitAmount { get; init; }

    /// <summary>Decreases what we owe the supplier (a payment we made).</summary>
    public required decimal CreditAmount { get; init; }

    public required decimal RunningBalance { get; init; }

    /// <summary>True for a payment that was later reversed — shown for audit transparency but contributes 0 to CreditAmount/RunningBalance.</summary>
    public required bool IsReversed { get; init; }
}

public sealed class SupplierLedgerDto
{
    public required long SupplierId { get; init; }

    public required string SupplierName { get; init; }

    public required decimal ClosingBalance { get; init; }

    public required IReadOnlyList<SupplierLedgerEntryDto> Entries { get; init; }
}

public sealed class SupplierAgingBucketDto
{
    public required long SupplierId { get; init; }

    public required string SupplierName { get; init; }

    /// <summary>Not yet due.</summary>
    public required decimal Current { get; init; }

    public required decimal Days1To30 { get; init; }

    public required decimal Days31To60 { get; init; }

    public required decimal Days61To90 { get; init; }

    public required decimal Over90Days { get; init; }

    public required decimal Total { get; init; }
}

public sealed class SupplierAgingReportDto
{
    public required DateTime GeneratedAtUtc { get; init; }

    public required IReadOnlyList<SupplierAgingBucketDto> Suppliers { get; init; }

    public required decimal GrandTotal { get; init; }
}
