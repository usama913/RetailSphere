using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var updateResult = product.UpdateDetails(
            request.Name,
            request.Description,
            request.CategoryId,
            request.BrandId,
            request.UnitId,
            request.ManageStock,
            request.NotForSelling);
        if (updateResult.IsFailure)
            return Result.Failure<ProductDto>(updateResult.Error);

        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "Updated", $"Updated product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
