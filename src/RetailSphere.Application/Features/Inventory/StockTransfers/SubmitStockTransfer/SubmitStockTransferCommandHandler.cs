using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.SubmitStockTransfer;

public sealed class SubmitStockTransferCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<SubmitStockTransferCommand, Result<StockTransferDto>>
{
    public async Task<Result<StockTransferDto>> Handle(SubmitStockTransferCommand request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        var submitResult = stockTransfer.Submit();
        if (submitResult.IsFailure)
            return Result.Failure<StockTransferDto>(submitResult.Error);

        stockTransferRepository.Update(stockTransfer);
        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "Submitted", $"Submitted stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }
}
