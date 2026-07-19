using MediatR;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.CreateSalesOrder;

public sealed record CreateSalesOrderLineItem(
    long ProductId,
    long ProductVariantId,
    decimal Quantity,
    decimal DiscountAmount);

/// <summary>
/// The POS checkout command — builds and finalizes a whole SalesOrder in one request
/// (see the class remarks on the SalesOrder aggregate).
///
/// OverrideCreditLimit: explicit cashier/manager acknowledgment to proceed anyway
/// when this sale would push the customer's outstanding balance over their credit
/// limit. The first checkout attempt always sends this as false; if the handler
/// rejects it with SalesOrder.CreditLimitExceeded, the POS shows a warning dialog
/// and only resubmits with this set to true after the user confirms — and even
/// then, only a user holding the sales.credit.override_limit permission can
/// actually succeed (see CreateSalesOrderCommandHandler).
/// </summary>
public sealed record CreateSalesOrderCommand(
    long BranchId,
    long? CustomerId,
    string? PaymentMethod,
    decimal OrderDiscountAmount,
    decimal AmountPaid,
    string? Notes,
    string? PaymentTerms,
    DateTime? DueDate,
    bool OverrideCreditLimit,
    IReadOnlyList<CreateSalesOrderLineItem> Lines) : IRequest<Result<SalesOrderDto>>;
