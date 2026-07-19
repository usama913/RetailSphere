using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Sales;

/// <summary>
/// Aggregate root: Sales Order — one Point-of-Sale transaction. Unlike PurchaseOrder
/// (Draft -> Submitted -> Received, built up over several requests), a sale is created
/// and finalized in a single checkout action: CreateSalesOrderCommandHandler builds the
/// whole aggregate — Create, then AddLine per cart item, then SetAmountPaid — before
/// ever saving it, so "Completed" is the only status a persisted order is ever first
/// seen in. Cancel exists purely to void a mistaken sale after the fact and reverse
/// its stock (see CancelSalesOrderCommandHandler) — it does not model returns/partial
/// refunds; that belongs to the separate Returns &amp; Exchanges workflow.
///
/// BranchId/CustomerId/CashierUserId are plain columns, not EF navigations — Branch,
/// Customer, and User are independent aggregates, same rule as every other
/// cross-aggregate reference in this codebase (e.g. PurchaseOrder.SupplierId).
/// CustomerId is nullable: walk-in sales have no customer on file.
/// </summary>
public sealed class SalesOrder : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public static readonly IReadOnlyList<string> Statuses = ["Completed", "Cancelled"];

    public static readonly IReadOnlyList<string> PaymentMethods = ["Cash", "Card"];

    private readonly List<SalesOrderLine> _lines = [];

    public string OrderNumber { get; private set; } = default!;

    public long BranchId { get; private set; }

    public long? CustomerId { get; private set; }

    public long? CashierUserId { get; private set; }

    public string Status { get; private set; } = "Completed";

    public DateTime OrderDate { get; private set; }

    public string PaymentMethod { get; private set; } = "Cash";

    /// <summary>An extra discount applied to the whole sale (e.g. a manager override), on top of any per-line discount.</summary>
    public decimal OrderDiscountAmount { get; private set; }

    public decimal AmountPaid { get; private set; }

    public string? Notes { get; private set; }

    public string? CancellationReason { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    public IReadOnlyCollection<SalesOrderLine> Lines => _lines.AsReadOnly();

    public decimal SubtotalAmount => _lines.Sum(l => l.UnitPrice * l.Quantity);

    public decimal LinesDiscountTotal => _lines.Sum(l => l.DiscountAmount);

    public decimal TaxAmount => _lines.Sum(l => l.TaxAmount);

    public decimal TotalAmount => Math.Max(0, _lines.Sum(l => l.LineTotal) - OrderDiscountAmount);

    public decimal ChangeDue => Math.Max(0, AmountPaid - TotalAmount);

    private SalesOrder()
    {
    }

    public static Result<SalesOrder> Create(
        string orderNumber,
        long branchId,
        long? customerId,
        long? cashierUserId,
        DateTime orderDate,
        string? paymentMethod,
        decimal orderDiscountAmount,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            return Result.Failure<SalesOrder>(Error.Validation("SalesOrder.NumberRequired", "Order number is required."));

        if (orderDiscountAmount < 0)
            return Result.Failure<SalesOrder>(Error.Validation("SalesOrder.InvalidDiscount", "Discount cannot be negative."));

        return Result.Success(new SalesOrder
        {
            OrderNumber = orderNumber.Trim(),
            BranchId = branchId,
            CustomerId = customerId,
            CashierUserId = cashierUserId,
            Status = "Completed",
            OrderDate = orderDate,
            PaymentMethod = NormalizePaymentMethod(paymentMethod),
            OrderDiscountAmount = orderDiscountAmount,
            Notes = notes,
        });
    }

    public Result<SalesOrderLine> AddLine(
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantity,
        decimal unitPrice,
        decimal taxRateSnapshot,
        string? taxTypeSnapshot,
        decimal discountAmount,
        decimal costPriceSnapshot = 0)
    {
        if (quantity <= 0)
            return Result.Failure<SalesOrderLine>(Error.Validation("SalesOrder.InvalidQuantity", "Quantity must be greater than zero."));

        if (unitPrice < 0)
            return Result.Failure<SalesOrderLine>(Error.Validation("SalesOrder.InvalidUnitPrice", "Unit price cannot be negative."));

        if (discountAmount < 0)
            return Result.Failure<SalesOrderLine>(Error.Validation("SalesOrder.InvalidDiscount", "Discount cannot be negative."));

        var line = SalesOrderLine.Create(Id, productId, productVariantId, skuSnapshot, descriptionSnapshot, quantity, unitPrice, taxRateSnapshot, taxTypeSnapshot, discountAmount, costPriceSnapshot);
        _lines.Add(line);
        return Result.Success(line);
    }

    /// <summary>
    /// Called once, after every line has been added, so TotalAmount reflects the
    /// finished cart. Rejects a payment that doesn't cover the total — this system
    /// doesn't model partial/on-account payment for POS sales (a future Customer
    /// credit/AR feature would need to relax this deliberately, not accidentally).
    /// </summary>
    public Result SetAmountPaid(decimal amountPaid)
    {
        if (amountPaid < 0)
            return Result.Failure(Error.Validation("SalesOrder.InvalidAmountPaid", "Amount paid cannot be negative."));

        if (amountPaid < TotalAmount)
            return Result.Failure(Error.Validation("SalesOrder.InsufficientPayment", "Amount paid is less than the order total."));

        AmountPaid = amountPaid;
        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status != "Completed")
            return Result.Failure(Error.Conflict("SalesOrder.InvalidStatus", "Only completed sales orders can be cancelled."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation("SalesOrder.CancellationReasonRequired", "A reason is required to cancel a sale."));

        Status = "Cancelled";
        CancellationReason = reason.Trim();
        return Result.Success();
    }

    private static string NormalizePaymentMethod(string? paymentMethod) =>
        !string.IsNullOrWhiteSpace(paymentMethod) && PaymentMethods.Contains(paymentMethod) ? paymentMethod : "Cash";
}
