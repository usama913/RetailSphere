using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.CreateStockTransfer;

public sealed class CreateStockTransferCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<CreateStockTransferCommand, Result<StockTransferDto>>
{
    private const int MaxTransferNumberAttempts = 5;

    public async Task<Result<StockTransferDto>> Handle(CreateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var fromBranch = await branchRepository.GetByIdAsync(request.FromBranchId, cancellationToken);
        if (fromBranch is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("Branch.NotFound", "Source branch not found."));

        var toBranch = await branchRepository.GetByIdAsync(request.ToBranchId, cancellationToken);
        if (toBranch is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("Branch.NotFound", "Destination branch not found."));

        // Same check-then-insert race + recovery pattern as
        // CreatePurchaseOrderCommandHandler.GeneratePoNumberAsync — see its remarks.
        StockTransfer stockTransfer;
        var attempt = 1;
        while (true)
        {
            var transferNumber = await GenerateTransferNumberAsync(cancellationToken);

            var createResult = StockTransfer.Create(transferNumber, request.FromBranchId, request.ToBranchId, request.TransferDate, request.Notes);
            if (createResult.IsFailure)
                return Result.Failure<StockTransferDto>(createResult.Error);

            stockTransfer = createResult.Value;
            stockTransferRepository.Add(stockTransfer);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (Exception ex) when (attempt < MaxTransferNumberAttempts && IsDuplicateTransferNumberViolation(ex))
            {
                stockTransferRepository.Remove(stockTransfer);
                attempt++;
            }
        }

        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "Created", $"Created stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }

    private static bool IsDuplicateTransferNumberViolation(Exception ex) =>
        ex.GetType().Name == "DbUpdateException"
        && (ex.InnerException?.Message.Contains("IX_StockTransfers_TransferNumber", StringComparison.OrdinalIgnoreCase) == true
            || ex.InnerException?.Message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase) == true);

    /// <summary>Auto-generates a month-scoped transfer number (e.g. "TR-202607-0001") — mirrors GeneratePoNumberAsync.</summary>
    private async Task<string> GenerateTransferNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = $"TR-{DateTime.UtcNow:yyyyMM}-";
        var sequence = 1;
        while (true)
        {
            var candidate = $"{prefix}{sequence:0000}";
            if (!await stockTransferRepository.TransferNumberExistsAsync(candidate, cancellationToken))
                return candidate;

            sequence++;
        }
    }
}
