using MediatR;
using RetailSphere.Application.Features.SalesReturns.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesReturns.GetSalesReturns;

public sealed class GetSalesReturnsQueryHandler(ISalesReturnRepository salesReturnRepository, SalesReturnDtoAssembler salesReturnDtoAssembler)
    : IRequestHandler<GetSalesReturnsQuery, Result<PagedResult<SalesReturnDto>>>
{
    public async Task<Result<PagedResult<SalesReturnDto>>> Handle(GetSalesReturnsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await salesReturnRepository.SearchAsync(
            request.Page, request.PageSize, request.BranchId, request.CustomerId, request.SalesOrderId, request.FromDate, request.ToDate, cancellationToken);

        var dtos = await salesReturnDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<SalesReturnDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
