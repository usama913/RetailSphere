using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.GetSalesOrders;

public sealed record GetSalesOrdersQuery(
    int Page,
    int PageSize,
    string? Search,
    long? BranchId,
    long? CustomerId,
    string? Status,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<Result<PagedResult<SalesOrderDto>>>;
