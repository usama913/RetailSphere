using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.DeactivateProduct;

public sealed class DeactivateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateProductCommand, Result>
{
    public async Task<Result> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", "Product not found."));

        product.Deactivate();
        productRepository.Update(product);
        auditLogService.Log("Product", product.Id.ToString(), "Deactivated", $"Deactivated product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
