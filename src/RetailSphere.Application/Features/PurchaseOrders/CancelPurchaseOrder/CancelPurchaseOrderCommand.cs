using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.CancelPurchaseOrder;

public sealed record CancelPurchaseOrderCommand(long Id) : IRequest<Result<PurchaseOrderDto>>;
