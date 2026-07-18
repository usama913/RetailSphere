using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrder;

public sealed record UpdatePurchaseOrderCommand(
    long Id,
    long SupplierId,
    long BranchId,
    DateTime OrderDate,
    DateTime? ExpectedDeliveryDate,
    string? Notes) : IRequest<Result<PurchaseOrderDto>>;
