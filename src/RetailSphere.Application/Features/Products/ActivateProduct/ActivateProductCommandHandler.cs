using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.ActivateProduct;

public sealed class ActivateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ActivateProductCommand, Result>
{
    public async Task<Result> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", "Product not found."));

        product.Activate();
        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "Activated", $"Activated product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
