using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransfer;

public sealed class UpdateStockTransferCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<UpdateStockTransferCommand, Result<StockTransferDto>>
{
    public async Task<Result<StockTransferDto>> Handle(UpdateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        var fromBranch = await branchRepository.GetByIdAsync(request.FromBranchId, cancellationToken);
        if (fromBranch is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("Branch.NotFound", "Source branch not found."));

        var toBranch = await branchRepository.GetByIdAsync(request.ToBranchId, cancellationToken);
        if (toBranch is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("Branch.NotFound", "Destination branch not found."));

        var updateResult = stockTransfer.UpdateDetails(request.FromBranchId, request.ToBranchId, request.TransferDate, request.Notes);
        if (updateResult.IsFailure)
            return Result.Failure<StockTransferDto>(updateResult.Error);

        stockTransferRepository.Update(stockTransfer);
        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "Updated", $"Updated stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }
}
