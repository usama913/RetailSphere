using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.PurchaseOrders.Common;

/// <summary>Resolves the SupplierName/BranchName lookups PurchaseOrderDto needs on top of PurchaseOrder's own scalar fields — mirrors ProductDtoAssembler.</summary>
public sealed class PurchaseOrderDtoAssembler(ISupplierRepository supplierRepository, IBranchRepository branchRepository)
{
    public async Task<PurchaseOrderDto> ToDtoAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        var supplier = await supplierRepository.GetByIdAsync(purchaseOrder.SupplierId, cancellationToken);
        var branch = await branchRepository.GetByIdAsync(purchaseOrder.BranchId, cancellationToken);

        return PurchaseOrderMappings.ToDto(purchaseOrder, supplier?.Name, branch?.Name);
    }

    public async Task<IReadOnlyList<PurchaseOrderDto>> ToDtosAsync(IEnumerable<PurchaseOrder> purchaseOrders, CancellationToken cancellationToken = default)
    {
        var suppliers = (await supplierRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(s => s.Id, s => s.Name);
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        return purchaseOrders
            .Select(po => PurchaseOrderMappings.ToDto(
                po,
                suppliers.TryGetValue(po.SupplierId, out var supplierName) ? supplierName : null,
                branches.TryGetValue(po.BranchId, out var branchName) ? branchName : null))
            .ToList();
    }
}
