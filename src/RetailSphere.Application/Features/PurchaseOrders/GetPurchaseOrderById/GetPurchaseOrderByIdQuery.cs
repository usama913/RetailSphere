using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.GetPurchaseOrderById;

public sealed record GetPurchaseOrderByIdQuery(long Id) : IRequest<Result<PurchaseOrderDto>>;
