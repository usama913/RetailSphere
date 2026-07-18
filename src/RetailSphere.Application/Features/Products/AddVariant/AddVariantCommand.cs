using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.AddVariant;

public sealed record AddVariantCommand(
    long ProductId,
    string? Sku,
    string? Barcode,
    string? BarcodeType,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    decimal TaxRate,
    string? TaxType,
    decimal? Weight,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    IReadOnlyList<long> AttributeValueIds) : IRequest<Result<ProductDto>>;
