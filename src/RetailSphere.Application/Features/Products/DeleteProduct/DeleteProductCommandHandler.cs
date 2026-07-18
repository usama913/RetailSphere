using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", "Product not found."));

        productRepository.Remove(product);
        auditLogService.Log("Product", product.Id.ToString(), "Deleted", $"Deleted product '{product.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
