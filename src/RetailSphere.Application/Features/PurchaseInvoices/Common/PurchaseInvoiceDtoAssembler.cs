using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.PurchaseInvoices.Common;

/// <summary>Resolves the SupplierName/BranchName/PurchaseOrderNumber lookups PurchaseInvoiceDto needs on top of PurchaseInvoice's own scalar fields — mirrors PurchaseOrderDtoAssembler.</summary>
public sealed class PurchaseInvoiceDtoAssembler(
    ISupplierRepository supplierRepository,
    IBranchRepository branchRepository,
    IPurchaseOrderRepository purchaseOrderRepository)
{
    public async Task<PurchaseInvoiceDto> ToDtoAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default)
    {
        var supplier = await supplierRepository.GetByIdAsync(invoice.SupplierId, cancellationToken);
        var branch = await branchRepository.GetByIdAsync(invoice.BranchId, cancellationToken);
        var purchaseOrder = invoice.PurchaseOrderId.HasValue
            ? await purchaseOrderRepository.GetByIdAsync(invoice.PurchaseOrderId.Value, cancellationToken)
            : null;

        return PurchaseInvoiceMappings.ToDto(invoice, supplier?.Name, branch?.Name, purchaseOrder?.PoNumber);
    }

    public async Task<IReadOnlyList<PurchaseInvoiceDto>> ToDtosAsync(IEnumerable<PurchaseInvoice> invoices, CancellationToken cancellationToken = default)
    {
        var items = invoices.ToList();

        var suppliers = (await supplierRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(s => s.Id, s => s.Name);
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        var purchaseOrderIds = items.Where(i => i.PurchaseOrderId.HasValue).Select(i => i.PurchaseOrderId!.Value).Distinct().ToList();
        var purchaseOrderNumbers = new Dictionary<long, string>();
        foreach (var poId in purchaseOrderIds)
        {
            var po = await purchaseOrderRepository.GetByIdAsync(poId, cancellationToken);
            if (po is not null)
                purchaseOrderNumbers[poId] = po.PoNumber;
        }

        return items
            .Select(invoice => PurchaseInvoiceMappings.ToDto(
                invoice,
                suppliers.TryGetValue(invoice.SupplierId, out var supplierName) ? supplierName : null,
                branches.TryGetValue(invoice.BranchId, out var branchName) ? branchName : null,
                invoice.PurchaseOrderId.HasValue && purchaseOrderNumbers.TryGetValue(invoice.PurchaseOrderId.Value, out var poNumber) ? poNumber : null))
            .ToList();
    }
}
