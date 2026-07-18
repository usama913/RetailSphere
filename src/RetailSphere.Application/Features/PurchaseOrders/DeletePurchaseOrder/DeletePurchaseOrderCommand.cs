using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.DeletePurchaseOrder;

public sealed record DeletePurchaseOrderCommand(long Id) : IRequest<Result>;
