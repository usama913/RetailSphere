using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(
            request.Name,
            request.Description,
            request.CategoryId,
            request.BrandId,
            request.UnitId,
            request.ManageStock,
            request.NotForSelling);
        if (productResult.IsFailure)
            return Result.Failure<ProductDto>(productResult.Error);

        var product = productResult.Value;
        productRepository.Add(product);

        // Save once first so the auto-increment Id exists before it's referenced in the audit entry.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Product", product.Id.ToString(), "Created", $"Created product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
