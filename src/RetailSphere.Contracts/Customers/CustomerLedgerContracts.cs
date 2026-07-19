namespace RetailSphere.Contracts.Customers;

public sealed class CustomerLedgerEntryDto
{
    public required DateTime Date { get; init; }

    /// <summary>"Sale" or "Payment".</summary>
    public required string Type { get; init; }

    public string? ReferenceNumber { get; init; }

    public required string Description { get; init; }

    /// <summary>Increases what the customer owes us (a new sale on credit).</summary>
    public required decimal DebitAmount { get; init; }

    /// <summary>Decreases what the customer owes us (a payment they made).</summary>
    public required decimal CreditAmount { get; init; }

    public required decimal RunningBalance { get; init; }

    /// <summary>True for a payment that was later reversed — shown for audit transparency but contributes 0 to CreditAmount/RunningBalance.</summary>
    public required bool IsReversed { get; init; }
}

public sealed class CustomerLedgerDto
{
    public required long CustomerId { get; init; }

    public required string CustomerName { get; init; }

    public required decimal ClosingBalance { get; init; }

    public required IReadOnlyList<CustomerLedgerEntryDto> Entries { get; init; }
}

public sealed class CustomerAgingBucketDto
{
    public required long CustomerId { get; init; }

    public required string CustomerName { get; init; }

    public required decimal Current { get; init; }

    public required decimal Days1To30 { get; init; }

    public required decimal Days31To60 { get; init; }

    public required decimal Days61To90 { get; init; }

    public required decimal Over90Days { get; init; }

    public required decimal Total { get; init; }
}

public sealed class CustomerAgingReportDto
{
    public required DateTime GeneratedAtUtc { get; init; }

    public required IReadOnlyList<CustomerAgingBucketDto> Customers { get; init; }

    public required decimal GrandTotal { get; init; }
}
