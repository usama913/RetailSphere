namespace RetailSphere.Contracts.Customers;

/// <summary>
/// What the POS checkout screen shows the moment a customer is selected — current
/// standing at a glance, so a cashier can decide whether to allow a credit/split
/// sale before ringing it up (per the POS Integration requirements).
/// </summary>
public sealed class CustomerCreditSummaryDto
{
    public required long CustomerId { get; init; }

    public required string CustomerName { get; init; }

    public decimal? CreditLimit { get; init; }

    public required decimal CurrentOutstandingBalance { get; init; }

    /// <summary>Null when the customer has no credit limit set (unlimited).</summary>
    public decimal? AvailableCredit { get; init; }

    public required int UnpaidInvoiceCount { get; init; }

    public DateTime? LastPaymentDate { get; init; }

    public required decimal TotalOverdueAmount { get; init; }

    /// <summary>True when CurrentOutstandingBalance already exceeds CreditLimit — the checkout screen should warn (and, per settings, allow an authorized override) before completing a new credit/split sale.</summary>
    public required bool IsOverCreditLimit { get; init; }
}
