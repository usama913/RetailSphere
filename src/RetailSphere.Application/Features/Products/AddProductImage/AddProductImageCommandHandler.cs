using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.AddProductImage;

public sealed class AddProductImageCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<AddProductImageCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var addResult = product.AddImage(request.Url);
        if (addResult.IsFailure)
            return Result.Failure<ProductDto>(addResult.Error);

        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "ImageAdded", $"Added an image to product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
