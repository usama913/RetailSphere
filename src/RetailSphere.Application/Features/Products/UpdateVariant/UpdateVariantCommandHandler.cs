using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Catalog.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.UpdateVariant;

public sealed class UpdateVariantCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<UpdateVariantCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

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

        var updateResult = product.UpdateVariant(
            request.VariantId,
            priceResult.Value,
            compareAtPrice,
            costPrice,
            request.TaxRate,
            request.TaxType,
            barcode,
            request.BarcodeType,
            request.UnitOfMeasure,
            request.Weight,
            request.Length,
            request.Width,
            request.Height,
            request.AttributeValueIds);
        if (updateResult.IsFailure)
            return Result.Failure<ProductDto>(updateResult.Error);

        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "VariantUpdated", $"Updated a variant on product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
