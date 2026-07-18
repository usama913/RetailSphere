using MediatR;
using RetailSphere.Application.Features.Inventory.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.GetStockItemById;

public sealed class GetStockItemByIdQueryHandler(IStockItemRepository stockItemRepository, StockItemDtoAssembler stockItemDtoAssembler)
    : IRequestHandler<GetStockItemByIdQuery, Result<StockItemDto>>
{
    public async Task<Result<StockItemDto>> Handle(GetStockItemByIdQuery request, CancellationToken cancellationToken)
    {
        var stockItem = await stockItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockItem is null)
            return Result.Failure<StockItemDto>(Error.NotFound("StockItem.NotFound", "Stock item not found."));

        var dto = await stockItemDtoAssembler.ToDtoAsync(stockItem, cancellationToken);
        return Result.Success(dto);
    }
}
