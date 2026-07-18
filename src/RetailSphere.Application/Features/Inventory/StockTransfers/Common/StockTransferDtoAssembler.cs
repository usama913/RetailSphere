using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.Common;

/// <summary>Resolves the FromBranchName/ToBranchName lookups StockTransferDto needs on top of StockTransfer's own scalar fields — mirrors PurchaseOrderDtoAssembler.</summary>
public sealed class StockTransferDtoAssembler(IBranchRepository branchRepository)
{
    public async Task<StockTransferDto> ToDtoAsync(StockTransfer stockTransfer, CancellationToken cancellationToken = default)
    {
        var fromBranch = await branchRepository.GetByIdAsync(stockTransfer.FromBranchId, cancellationToken);
        var toBranch = await branchRepository.GetByIdAsync(stockTransfer.ToBranchId, cancellationToken);

        return StockTransferMappings.ToDto(stockTransfer, fromBranch?.Name, toBranch?.Name);
    }

    public async Task<IReadOnlyList<StockTransferDto>> ToDtosAsync(IEnumerable<StockTransfer> stockTransfers, CancellationToken cancellationToken = default)
    {
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        return stockTransfers
            .Select(t => StockTransferMappings.ToDto(
                t,
                branches.TryGetValue(t.FromBranchId, out var fromName) ? fromName : null,
                branches.TryGetValue(t.ToBranchId, out var toName) ? toName : null))
            .ToList();
    }
}
