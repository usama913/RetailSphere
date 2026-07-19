using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Customers;

/// <summary>
/// Aggregate root: a payment received from a Customer (Accounts Receivable side).
/// A single payment can be split across multiple Sales Orders via Allocations —
/// e.g. a customer clears three unpaid invoices with one bank transfer.
///
/// Mirrors SupplierPayment's shape on the Accounts Payable side. Allocate/RemoveAllocation
/// only track how this payment's cash was distributed; the command handler is
/// responsible for mirroring each allocation onto the target SalesOrder's own
/// AmountPaid via RecordAdditionalPayment/ReverseAdditionalPayment within the same
/// SaveChangesAsync, exactly like SupplierPayment keeps PurchaseInvoice.AmountPaid in sync.
/// </summary>
public sealed class CustomerPayment : AggregateRoot<long>, IAuditableEntity
{
    public static readonly IReadOnlyList<string> PaymentMethods = ["Cash", "Card", "Bank Transfer", "Mobile Payment", "Cheque", "Other"];

    public long CustomerId { get; private set; }

    public long BranchId { get; private set; }

    public DateTime PaymentDate { get; private set; }

    /// <summary>Total amount received — may exceed the sum of Allocations if some of it hasn't been applied to a specific invoice yet (kept as an on-account credit).</summary>
    public decimal Amount { get; private set; }

    public string PaymentMethod { get; private set; } = "Cash";

    public string? ReferenceNumber { get; private set; }

    public string? Notes { get; private set; }

    public bool IsReversed { get; private set; }

    public string? ReversalReason { get; private set; }

    public DateTime? ReversedAtUtc { get; private set; }

    public long? ReversedByUserId { get; private set; }

    private readonly List<CustomerPaymentAllocation> _allocations = [];
    public IReadOnlyCollection<CustomerPaymentAllocation> Allocations => _allocations.AsReadOnly();

    public decimal AllocatedAmount => _allocations.Sum(a => a.Amount);

    /// <summary>Portion of this payment not yet applied to any Sales Order — still available to allocate.</summary>
    public decimal UnallocatedAmount => Math.Max(0, Amount - AllocatedAmount);

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    private CustomerPayment()
    {
    }

    public static Result<CustomerPayment> Create(
        long customerId,
        long branchId,
        DateTime paymentDate,
        decimal amount,
        string? paymentMethod,
        string? referenceNumber,
        string? notes)
    {
        if (amount <= 0)
            return Result.Failure<CustomerPayment>(Error.Validation("CustomerPayment.InvalidAmount", "Amount must be greater than zero."));

        return Result.Success(new CustomerPayment
        {
            CustomerId = customerId,
            BranchId = branchId,
            PaymentDate = paymentDate,
            Amount = amount,
            PaymentMethod = NormalizePaymentMethod(paymentMethod),
            ReferenceNumber = referenceNumber,
            Notes = notes,
        });
    }

    /// <summary>
    /// Edits this payment's header details. Amount can only change while nothing has
    /// been allocated yet (AllocatedAmount == 0) — once part of it has been applied to
    /// a Sales Order, shrinking or growing Amount out from under those allocations
    /// would leave AllocatedAmount/UnallocatedAmount inconsistent, so remove the
    /// allocations first (or reverse the payment) to correct it instead.
    /// </summary>
    public Result UpdateDetails(DateTime paymentDate, decimal amount, string? paymentMethod, string? referenceNumber, string? notes)
    {
        if (IsReversed)
            return Result.Failure(Error.Conflict("CustomerPayment.Reversed", "A reversed payment cannot be edited."));

        if (amount <= 0)
            return Result.Failure(Error.Validation("CustomerPayment.InvalidAmount", "Amount must be greater than zero."));

        if (amount != Amount && AllocatedAmount > 0)
            return Result.Failure(Error.Conflict("CustomerPayment.HasAllocations", "This payment is already allocated to one or more sales orders — remove those allocations before changing its amount."));

        PaymentDate = paymentDate;
        Amount = amount;
        PaymentMethod = NormalizePaymentMethod(paymentMethod);
        ReferenceNumber = referenceNumber;
        Notes = notes;
        return Result.Success();
    }

    /// <summary>Applies part of this payment's still-unallocated amount to one Sales Order. The caller must mirror this same amount onto that SalesOrder via RecordAdditionalPayment in the same unit of work.</summary>
    public Result Allocate(long salesOrderId, decimal amount)
    {
        if (IsReversed)
            return Result.Failure(Error.Conflict("CustomerPayment.Reversed", "A reversed payment cannot be allocated."));

        if (amount <= 0)
            return Result.Failure(Error.Validation("CustomerPayment.InvalidAllocationAmount", "Allocation amount must be greater than zero."));

        if (amount > UnallocatedAmount)
            return Result.Failure(Error.Validation("CustomerPayment.AllocationExceedsUnallocated", "Allocation amount exceeds this payment's unallocated balance."));

        if (_allocations.Any(a => a.SalesOrderId == salesOrderId))
            return Result.Failure(Error.Conflict("CustomerPayment.AlreadyAllocated", "This payment is already allocated to that sales order — remove the existing allocation first to change it."));

        _allocations.Add(CustomerPaymentAllocation.Create(Id, salesOrderId, amount));
        return Result.Success();
    }

    /// <summary>Removes a previously-made allocation. Returns the removed amount so the caller can reverse the same amount off the SalesOrder via ReverseAdditionalPayment.</summary>
    public Result<decimal> RemoveAllocation(long salesOrderId)
    {
        if (IsReversed)
            return Result.Failure<decimal>(Error.Conflict("CustomerPayment.Reversed", "A reversed payment's allocations cannot be changed."));

        var allocation = _allocations.FirstOrDefault(a => a.SalesOrderId == salesOrderId);
        if (allocation is null)
            return Result.Failure<decimal>(Error.NotFound("CustomerPayment.AllocationNotFound", "No allocation exists for that sales order on this payment."));

        _allocations.Remove(allocation);
        return Result.Success(allocation.Amount);
    }

    /// <summary>
    /// Marks the whole payment reversed. Allocations are left in place as a historical
    /// record (the ledger shows them struck through) — the command handler must walk
    /// Allocations before/at reversal time and call ReverseAdditionalPayment on each
    /// affected SalesOrder for the same amount.
    /// </summary>
    public Result Reverse(string reason, long? reversedByUserId)
    {
        if (IsReversed)
            return Result.Failure(Error.Conflict("CustomerPayment.AlreadyReversed", "This payment has already been reversed."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation("CustomerPayment.ReversalReasonRequired", "A reason is required to reverse a payment."));

        IsReversed = true;
        ReversalReason = reason.Trim();
        ReversedAtUtc = DateTime.UtcNow;
        ReversedByUserId = reversedByUserId;
        return Result.Success();
    }

    private static string NormalizePaymentMethod(string? paymentMethod) =>
        !string.IsNullOrWhiteSpace(paymentMethod) && PaymentMethods.Contains(paymentMethod) ? paymentMethod : "Cash";
}
