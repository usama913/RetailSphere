using MediatR;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.GetStockTransferById;

public sealed class GetStockTransferByIdQueryHandler(IStockTransferRepository stockTransferRepository, StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<GetStockTransferByIdQuery, Result<StockTransferDto>>
{
    public async Task<Result<StockTransferDto>> Handle(GetStockTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }
}
