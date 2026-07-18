using MediatR;
using RetailSphere.Application.Features.SalesOrders.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.GetSalesOrders;

public sealed class GetSalesOrdersQueryHandler(ISalesOrderRepository salesOrderRepository, SalesOrderDtoAssembler salesOrderDtoAssembler)
    : IRequestHandler<GetSalesOrdersQuery, Result<PagedResult<SalesOrderDto>>>
{
    public async Task<Result<PagedResult<SalesOrderDto>>> Handle(GetSalesOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await salesOrderRepository.SearchAsync(
            request.Page, request.PageSize, request.Search, request.BranchId, request.CustomerId, request.Status, request.FromDate, request.ToDate, cancellationToken);

        var dtos = await salesOrderDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<SalesOrderDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
