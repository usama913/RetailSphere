using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Purchasing;

/// <summary>
/// Aggregate root: a payment we made against one Purchase Invoice. Kept as its own
/// aggregate (not a child of PurchaseInvoice) because it needs an independent audit
/// trail — creation, edits, and reversals are each separately logged (see
/// AuditLogService usage in the SupplierPayments command handlers) the same way
/// StockAdjustment is independent of StockItem.
///
/// Every mutation that changes Amount (Create, UpdateDetails, Reverse) is mirrored
/// by a call into the target PurchaseInvoice's ApplyPayment/ReversePayment — the two
/// aggregates are kept in sync by the command handler within a single SaveChangesAsync,
/// not by a foreign-key cascade or a computed EF view.
/// </summary>
public sealed class SupplierPayment : AggregateRoot<long>, IAuditableEntity
{
    public static readonly IReadOnlyList<string> PaymentMethods = ["Cash", "Card", "Bank Transfer", "Cheque", "Other"];

    public long SupplierId { get; private set; }

    public long PurchaseInvoiceId { get; private set; }

    public long BranchId { get; private set; }

    public DateTime PaymentDate { get; private set; }

    public decimal Amount { get; private set; }

    public string PaymentMethod { get; private set; } = "Cash";

    public string? ReferenceNumber { get; private set; }

    public string? Notes { get; private set; }

    public bool IsReversed { get; private set; }

    public string? ReversalReason { get; private set; }

    public DateTime? ReversedAtUtc { get; private set; }

    public long? ReversedByUserId { get; private set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    private SupplierPayment()
    {
    }

    public static Result<SupplierPayment> Create(
        long supplierId,
        long purchaseInvoiceId,
        long branchId,
        DateTime paymentDate,
        decimal amount,
        string? paymentMethod,
        string? referenceNumber,
        string? notes)
    {
        if (amount <= 0)
            return Result.Failure<SupplierPayment>(Error.Validation("SupplierPayment.InvalidAmount", "Amount must be greater than zero."));

        return Result.Success(new SupplierPayment
        {
            SupplierId = supplierId,
            PurchaseInvoiceId = purchaseInvoiceId,
            BranchId = branchId,
            PaymentDate = paymentDate,
            Amount = amount,
            PaymentMethod = NormalizePaymentMethod(paymentMethod),
            ReferenceNumber = referenceNumber,
            Notes = notes,
        });
    }

    /// <summary>
    /// Returns the previous Amount so the command handler can apply the delta
    /// (newAmount - oldAmount) to the linked PurchaseInvoice's balance.
    /// </summary>
    public Result<decimal> UpdateDetails(DateTime paymentDate, decimal amount, string? paymentMethod, string? referenceNumber, string? notes)
    {
        if (IsReversed)
            return Result.Failure<decimal>(Error.Conflict("SupplierPayment.Reversed", "A reversed payment cannot be edited."));

        if (amount <= 0)
            return Result.Failure<decimal>(Error.Validation("SupplierPayment.InvalidAmount", "Amount must be greater than zero."));

        var previousAmount = Amount;
        PaymentDate = paymentDate;
        Amount = amount;
        PaymentMethod = NormalizePaymentMethod(paymentMethod);
        ReferenceNumber = referenceNumber;
        Notes = notes;
        return Result.Success(previousAmount);
    }

    public Result Reverse(string reason, long? reversedByUserId)
    {
        if (IsReversed)
            return Result.Failure(Error.Conflict("SupplierPayment.AlreadyReversed", "This payment has already been reversed."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation("SupplierPayment.ReversalReasonRequired", "A reason is required to reverse a payment."));

        IsReversed = true;
        ReversalReason = reason.Trim();
        ReversedAtUtc = DateTime.UtcNow;
        ReversedByUserId = reversedByUserId;
        return Result.Success();
    }

    private static string NormalizePaymentMethod(string? paymentMethod) =>
        !string.IsNullOrWhiteSpace(paymentMethod) && PaymentMethods.Contains(paymentMethod) ? paymentMethod : "Cash";
}
