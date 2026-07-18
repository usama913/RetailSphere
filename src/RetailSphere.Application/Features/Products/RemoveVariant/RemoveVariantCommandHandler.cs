using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.RemoveVariant;

public sealed class RemoveVariantCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<RemoveVariantCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(RemoveVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var removeResult = product.RemoveVariant(request.VariantId);
        if (removeResult.IsFailure)
            return Result.Failure<ProductDto>(removeResult.Error);

        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "VariantRemoved", $"Removed a variant from product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
