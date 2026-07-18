using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.RemovePurchaseOrderLine;

public sealed record RemovePurchaseOrderLineCommand(long PurchaseOrderId, long LineId) : IRequest<Result<PurchaseOrderDto>>;
