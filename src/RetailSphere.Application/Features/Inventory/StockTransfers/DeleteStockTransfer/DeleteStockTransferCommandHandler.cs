using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.DeleteStockTransfer;

public sealed class DeleteStockTransferCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteStockTransferCommand, Result>
{
    public async Task<Result> Handle(DeleteStockTransferCommand request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        if (stockTransfer.Status != "Draft")
            return Result.Failure(Error.Conflict("StockTransfer.NotDeletable", "Only draft transfers can be deleted — cancel it instead."));

        stockTransferRepository.Remove(stockTransfer);
        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "Deleted", $"Deleted stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
