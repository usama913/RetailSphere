using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrderLine;

public sealed record UpdatePurchaseOrderLineCommand(
    long PurchaseOrderId,
    long LineId,
    decimal QuantityOrdered,
    decimal UnitCost) : IRequest<Result<PurchaseOrderDto>>;
