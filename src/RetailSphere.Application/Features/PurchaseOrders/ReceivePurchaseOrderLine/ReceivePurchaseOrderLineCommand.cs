using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.ReceivePurchaseOrderLine;

public sealed record ReceivePurchaseOrderLineCommand(long PurchaseOrderId, long LineId, decimal Quantity) : IRequest<Result<PurchaseOrderDto>>;
