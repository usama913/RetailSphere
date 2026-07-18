using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.CreatePurchaseOrder;

public sealed record CreatePurchaseOrderCommand(
    long SupplierId,
    long BranchId,
    DateTime OrderDate,
    DateTime? ExpectedDeliveryDate,
    string? Notes) : IRequest<Result<PurchaseOrderDto>>;
