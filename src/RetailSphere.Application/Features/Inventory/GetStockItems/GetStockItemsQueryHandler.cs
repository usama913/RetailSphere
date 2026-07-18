using MediatR;
using RetailSphere.Application.Features.Inventory.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.GetStockItems;

public sealed class GetStockItemsQueryHandler(IStockItemRepository stockItemRepository, StockItemDtoAssembler stockItemDtoAssembler)
    : IRequestHandler<GetStockItemsQuery, Result<PagedResult<StockItemDto>>>
{
    public async Task<Result<PagedResult<StockItemDto>>> Handle(GetStockItemsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await stockItemRepository.SearchAsync(
            request.Page, request.PageSize, request.BranchId, request.ProductId, cancellationToken);

        var dtos = await stockItemDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<StockItemDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
