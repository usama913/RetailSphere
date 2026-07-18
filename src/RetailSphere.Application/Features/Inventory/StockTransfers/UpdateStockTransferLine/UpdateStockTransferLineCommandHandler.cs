using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransferLine;

public sealed class UpdateStockTransferLineCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<UpdateStockTransferLineCommand, Result<StockTransferDto>>
{
    public async Task<Result<StockTransferDto>> Handle(UpdateStockTransferLineCommand request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.StockTransferId, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        var updateResult = stockTransfer.UpdateLine(request.LineId, request.QuantityRequested);
        if (updateResult.IsFailure)
            return Result.Failure<StockTransferDto>(updateResult.Error);

        stockTransferRepository.Update(stockTransfer);
        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "LineUpdated", $"Updated a line on stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }
}
