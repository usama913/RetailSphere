using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.UpdateVariant;

public sealed record UpdateVariantCommand(
    long ProductId,
    long VariantId,
    string? Barcode,
    string? BarcodeType,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    decimal TaxRate,
    string? TaxType,
    string? UnitOfMeasure,
    decimal? Weight,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    IReadOnlyList<long> AttributeValueIds) : IRequest<Result<ProductDto>>;
