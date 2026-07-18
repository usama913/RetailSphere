using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.AddPurchaseOrderLine;

public sealed record AddPurchaseOrderLineCommand(
    long PurchaseOrderId,
    long ProductId,
    long ProductVariantId,
    decimal QuantityOrdered,
    decimal UnitCost) : IRequest<Result<PurchaseOrderDto>>;
