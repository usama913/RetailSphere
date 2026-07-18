using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.GetPurchaseOrders;

public sealed record GetPurchaseOrdersQuery(
    int Page,
    int PageSize,
    string? Search,
    long? SupplierId,
    long? BranchId,
    string? Status) : IRequest<Result<PagedResult<PurchaseOrderDto>>>;
