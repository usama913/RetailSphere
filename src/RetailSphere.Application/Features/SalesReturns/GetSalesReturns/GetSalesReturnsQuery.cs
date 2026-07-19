using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesReturns.GetSalesReturns;

public sealed record GetSalesReturnsQuery(
    int Page,
    int PageSize,
    long? BranchId,
    long? CustomerId,
    long? SalesOrderId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<Result<PagedResult<SalesReturnDto>>>;
