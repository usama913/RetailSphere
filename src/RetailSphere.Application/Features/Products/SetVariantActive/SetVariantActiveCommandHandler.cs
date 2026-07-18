using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.SetVariantActive;

public sealed class SetVariantActiveCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<SetVariantActiveCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(SetVariantActiveCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var setResult = product.SetVariantActive(request.VariantId, request.IsActive);
        if (setResult.IsFailure)
            return Result.Failure<ProductDto>(setResult.Error);

        productRepository.Update(product);
        auditLogService.Log(
            "Product",
            product.Id.ToString(),
            request.IsActive ? "VariantActivated" : "VariantDeactivated",
            $"{(request.IsActive ? "Activated" : "Deactivated")} a variant on product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
