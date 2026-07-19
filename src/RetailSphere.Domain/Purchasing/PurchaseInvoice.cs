using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Purchasing;

/// <summary>
/// Aggregate root: Purchase Invoice — the Accounts Payable document a Supplier
/// bills us with. Deliberately separate from PurchaseOrder (which only models the
/// "receive stock into inventory" workflow, with no invoice/payment concept at all
/// — see PurchaseOrder's remarks): a Purchase Invoice can reference an existing
/// PurchaseOrder for traceability (PurchaseOrderId, optional) but doesn't have to
/// — some supplier bills (services, adjustments) never go through a stock receipt.
///
/// Doesn't carry its own line items — the physical goods/costs are already tracked
/// on the linked PurchaseOrder's lines when one exists; this aggregate only tracks
/// the financial side (what's owed, what's been paid, by when). AmountPaid is a
/// denormalized running total, mutated exclusively through ApplyPayment/ReversePayment
/// by SupplierPayment's command handlers — never set directly — so it can never
/// drift from the sum of that invoice's non-reversed SupplierPayments.
/// </summary>
public sealed class PurchaseInvoice : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public long SupplierId { get; private set; }

    public long BranchId { get; private set; }

    /// <summary>Optional traceability link to the Purchase Order this invoice bills for — null when the supplier bill doesn't correspond to a tracked stock receipt.</summary>
    public long? PurchaseOrderId { get; private set; }

    public string SupplierInvoiceNumber { get; private set; } = default!;

    public DateTime InvoiceDate { get; private set; }

    public DateTime DueDate { get; private set; }

    public string PaymentTerms { get; private set; } = "Cash";

    public decimal SubtotalAmount { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal AmountPaid { get; private set; }

    public string? Notes { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    public decimal TotalAmount => Math.Max(0, SubtotalAmount - DiscountAmount + TaxAmount);

    public decimal OutstandingBalance => Math.Max(0, TotalAmount - AmountPaid);

    /// <summary>"Paid" / "PartiallyPaid" / "Unpaid" / "Overdue" — computed, not stored, so it's always consistent with AmountPaid/DueDate and never needs a background job to keep it in sync.</summary>
    public string PaymentStatus
    {
        get
        {
            if (OutstandingBalance <= 0)
                return "Paid";

            if (DueDate.Date < DateTime.UtcNow.Date)
                return "Overdue";

            return AmountPaid > 0 ? "PartiallyPaid" : "Unpaid";
        }
    }

    private PurchaseInvoice()
    {
    }

    public static Result<PurchaseInvoice> Create(
        long supplierId,
        long branchId,
        long? purchaseOrderId,
        string supplierInvoiceNumber,
        DateTime invoiceDate,
        DateTime dueDate,
        string? paymentTerms,
        decimal subtotalAmount,
        decimal discountAmount,
        decimal taxAmount,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(supplierInvoiceNumber))
            return Result.Failure<PurchaseInvoice>(Error.Validation("PurchaseInvoice.SupplierInvoiceNumberRequired", "Supplier invoice number is required."));

        if (subtotalAmount < 0 || discountAmount < 0 || taxAmount < 0)
            return Result.Failure<PurchaseInvoice>(Error.Validation("PurchaseInvoice.InvalidAmount", "Amounts cannot be negative."));

        if (discountAmount > subtotalAmount)
            return Result.Failure<PurchaseInvoice>(Error.Validation("PurchaseInvoice.DiscountExceedsSubtotal", "Discount cannot exceed the subtotal."));

        return Result.Success(new PurchaseInvoice
        {
            SupplierId = supplierId,
            BranchId = branchId,
            PurchaseOrderId = purchaseOrderId,
            SupplierInvoiceNumber = supplierInvoiceNumber.Trim(),
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? "Cash" : paymentTerms.Trim(),
            SubtotalAmount = subtotalAmount,
            DiscountAmount = discountAmount,
            TaxAmount = taxAmount,
            Notes = notes,
        });
    }

    /// <summary>
    /// Once a payment has been recorded, the header amounts are locked (editing them
    /// out from under a running AmountPaid would silently corrupt the balance) —
    /// reverse the payment(s) first if the invoice truly needs correcting.
    /// </summary>
    public Result UpdateDetails(
        string supplierInvoiceNumber,
        DateTime invoiceDate,
        DateTime dueDate,
        string? paymentTerms,
        decimal subtotalAmount,
        decimal discountAmount,
        decimal taxAmount,
        string? notes)
    {
        if (AmountPaid > 0)
            return Result.Failure(Error.Conflict("PurchaseInvoice.HasPayments", "This invoice already has payments recorded — reverse them before editing its amounts."));

        if (string.IsNullOrWhiteSpace(supplierInvoiceNumber))
            return Result.Failure(Error.Validation("PurchaseInvoice.SupplierInvoiceNumberRequired", "Supplier invoice number is required."));

        if (subtotalAmount < 0 || discountAmount < 0 || taxAmount < 0)
            return Result.Failure(Error.Validation("PurchaseInvoice.InvalidAmount", "Amounts cannot be negative."));

        if (discountAmount > subtotalAmount)
            return Result.Failure(Error.Validation("PurchaseInvoice.DiscountExceedsSubtotal", "Discount cannot exceed the subtotal."));

        SupplierInvoiceNumber = supplierInvoiceNumber.Trim();
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? "Cash" : paymentTerms.Trim();
        SubtotalAmount = subtotalAmount;
        DiscountAmount = discountAmount;
        TaxAmount = taxAmount;
        Notes = notes;
        return Result.Success();
    }

    /// <summary>Called by SupplierPayment's Create/Update handlers — never invoked directly from the API.</summary>
    public Result ApplyPayment(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure(Error.Validation("PurchaseInvoice.InvalidPaymentAmount", "Payment amount must be greater than zero."));

        if (amount > OutstandingBalance)
            return Result.Failure(Error.Validation("PurchaseInvoice.PaymentExceedsBalance", "Payment amount exceeds this invoice's outstanding balance."));

        AmountPaid += amount;
        return Result.Success();
    }

    /// <summary>Called by SupplierPayment's Reverse/Update handlers to undo a previously-applied amount.</summary>
    public Result ReversePayment(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure(Error.Validation("PurchaseInvoice.InvalidPaymentAmount", "Reversal amount must be greater than zero."));

        AmountPaid = Math.Max(0, AmountPaid - amount);
        return Result.Success();
    }
}
