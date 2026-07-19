using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Catalog.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.AddVariant;

public sealed class AddVariantCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<AddVariantCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(AddVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var skuResult = await ResolveSkuAsync(product, request.Sku, cancellationToken);
        if (skuResult.IsFailure)
            return Result.Failure<ProductDto>(skuResult.Error);

        var priceResult = Money.Create(request.Price);
        if (priceResult.IsFailure)
            return Result.Failure<ProductDto>(priceResult.Error);

        Money? compareAtPrice = null;
        if (request.CompareAtPrice.HasValue)
        {
            var compareAtPriceResult = Money.Create(request.CompareAtPrice.Value);
            if (compareAtPriceResult.IsFailure)
                return Result.Failure<ProductDto>(compareAtPriceResult.Error);
            compareAtPrice = compareAtPriceResult.Value;
        }

        Barcode? barcode = null;
        if (!string.IsNullOrWhiteSpace(request.Barcode))
        {
            var barcodeResult = Barcode.Create(request.Barcode);
            if (barcodeResult.IsFailure)
                return Result.Failure<ProductDto>(barcodeResult.Error);
            barcode = barcodeResult.Value;
        }

        Money? costPrice = null;
        if (request.CostPrice.HasValue)
        {
            var costPriceResult = Money.Create(request.CostPrice.Value);
            if (costPriceResult.IsFailure)
                return Result.Failure<ProductDto>(costPriceResult.Error);
            costPrice = costPriceResult.Value;
        }

        var addResult = product.AddVariant(
            skuResult.Value,
            priceResult.Value,
            barcode,
            request.BarcodeType,
            compareAtPrice,
            costPrice,
            request.TaxRate,
            request.TaxType,
            request.Weight,
            request.Length,
            request.Width,
            request.Height,
            request.ReorderPoint,
            request.AttributeValueIds,
            request.ExpiryDate);
        if (addResult.IsFailure)
            return Result.Failure<ProductDto>(addResult.Error);

        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "VariantAdded", $"Added variant '{addResult.Value.Sku}' to product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }

    /// <summary>
    /// Auto-generates a SKU (e.g. "P1042-V01") when the caller doesn't supply
    /// one — the "SKU generation" capability called out in the architecture
    /// doc's Catalog phase. A manually supplied SKU is validated and checked for
    /// global uniqueness instead.
    /// </summary>
    private async Task<Result<Sku>> ResolveSkuAsync(Product product, string? requestedSku, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(requestedSku))
        {
            var skuResult = Sku.Create(requestedSku);
            if (skuResult.IsFailure)
                return skuResult;

            if (await productRepository.SkuExistsAsync(skuResult.Value.Value, cancellationToken))
                return Result.Failure<Sku>(Error.Conflict("Product.DuplicateSku", "This SKU is already in use."));

            return skuResult;
        }

        var sequence = product.Variants.Count + 1;
        while (true)
        {
            var candidate = $"P{product.Id}-V{sequence:00}";
            if (!await productRepository.SkuExistsAsync(candidate, cancellationToken))
                return Sku.Create(candidate);

            sequence++;
        }
    }
}
