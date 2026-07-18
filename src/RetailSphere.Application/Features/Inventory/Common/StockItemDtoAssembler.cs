using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Application.Features.Inventory.Common;

/// <summary>
/// Resolves the Sku/ProductName/BranchName lookups StockItemDto needs on top of
/// StockItem's own scalar fields — mirrors ProductDtoAssembler/PurchaseOrderDtoAssembler.
/// The variant -> owning-Product resolution goes through IProductRepository.GetByVariantIdsAsync
/// since StockItem only stores a plain ProductVariantId (see StockItem's class remarks).
/// </summary>
public sealed class StockItemDtoAssembler(IProductRepository productRepository, IBranchRepository branchRepository)
{
    public async Task<StockItemDto> ToDtoAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetByVariantIdsAsync([stockItem.ProductVariantId], cancellationToken);
        var (variant, product) = FindVariant(products, stockItem.ProductVariantId);

        var branch = await branchRepository.GetByIdAsync(stockItem.BranchId, cancellationToken);

        return StockItemMappings.ToDto(stockItem, variant?.Sku, product?.Id, product?.Name, branch?.Name, variant?.CostPrice, variant?.ReorderPoint);
    }

    public async Task<IReadOnlyList<StockItemDto>> ToDtosAsync(IEnumerable<StockItem> stockItems, CancellationToken cancellationToken = default)
    {
        var items = stockItems.ToList();

        var products = await productRepository.GetByVariantIdsAsync(items.Select(s => s.ProductVariantId), cancellationToken);
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        return items
            .Select(stockItem =>
            {
                var (variant, product) = FindVariant(products, stockItem.ProductVariantId);
                return StockItemMappings.ToDto(
                    stockItem,
                    variant?.Sku,
                    product?.Id,
                    product?.Name,
                    branches.TryGetValue(stockItem.BranchId, out var branchName) ? branchName : null,
                    variant?.CostPrice,
                    variant?.ReorderPoint);
            })
            .ToList();
    }

    private static (ProductVariant? Variant, Product? Product) FindVariant(IReadOnlyList<Product> products, long productVariantId)
    {
        foreach (var product in products)
        {
            var variant = product.Variants.FirstOrDefault(v => v.Id == productVariantId);
            if (variant is not null)
                return (variant, product);
        }

        return (null, null);
    }
}
