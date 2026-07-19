namespace RetailSphere.Contracts.Reporting;

/// <summary>Backs the Dashboard Improvements widgets (Total Supplier/Customer Outstanding, Overdue Payments/Receivables, Today's Collections/Payments, Cash Flow, A/R, A/P).</summary>
public sealed class FinancialSummaryDto
{
    public required DateTime GeneratedAtUtc { get; init; }

    /// <summary>Accounts Payable — total outstanding across all non-voided Purchase Invoices.</summary>
    public required decimal AccountsPayable { get; init; }

    /// <summary>Accounts Receivable — total outstanding across all non-cancelled credit/split Sales Orders.</summary>
    public required decimal AccountsReceivable { get; init; }

    public required decimal OverdueSupplierAmount { get; init; }

    public required int OverdueSupplierInvoiceCount { get; init; }

    public required decimal OverdueCustomerAmount { get; init; }

    public required int OverdueCustomerInvoiceCount { get; init; }

    /// <summary>Sum of non-reversed CustomerPayments recorded today.</summary>
    public required decimal TodaysCollections { get; init; }

    /// <summary>Sum of non-reversed SupplierPayments recorded today.</summary>
    public required decimal TodaysSupplierPayments { get; init; }

    /// <summary>TodaysCollections minus TodaysSupplierPayments — a same-day snapshot, not a full cash-position calculation.</summary>
    public required decimal NetCashFlowToday { get; init; }
}
