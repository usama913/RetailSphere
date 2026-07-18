using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.SubmitPurchaseOrder;

public sealed record SubmitPurchaseOrderCommand(long Id) : IRequest<Result<PurchaseOrderDto>>;
