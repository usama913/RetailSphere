using MediatR;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.GetStockTransfers;

public sealed class GetStockTransfersQueryHandler(IStockTransferRepository stockTransferRepository, StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<GetStockTransfersQuery, Result<PagedResult<StockTransferDto>>>
{
    public async Task<Result<PagedResult<StockTransferDto>>> Handle(GetStockTransfersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await stockTransferRepository.SearchAsync(
            request.Page, request.PageSize, request.Search, request.FromBranchId, request.ToBranchId, request.Status, cancellationToken);

        var dtos = await stockTransferDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<StockTransferDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
