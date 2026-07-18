using MediatR;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.CreateSalesOrder;

public sealed record CreateSalesOrderLineItem(
    long ProductId,
    long ProductVariantId,
    decimal Quantity,
    decimal DiscountAmount);

/// <summary>The POS checkout command — builds and finalizes a whole SalesOrder in one request (see the class remarks on the SalesOrder aggregate).</summary>
public sealed record CreateSalesOrderCommand(
    long BranchId,
    long? CustomerId,
    string? PaymentMethod,
    decimal OrderDiscountAmount,
    decimal AmountPaid,
    string? Notes,
    IReadOnlyList<CreateSalesOrderLineItem> Lines) : IRequest<Result<SalesOrderDto>>;
