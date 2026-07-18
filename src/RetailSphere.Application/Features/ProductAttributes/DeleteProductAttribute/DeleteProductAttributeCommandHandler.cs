using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.DeleteProductAttribute;

public sealed class DeleteProductAttributeCommandHandler(
    IProductAttributeRepository productAttributeRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteProductAttributeCommand, Result>
{
    public async Task<Result> Handle(DeleteProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attribute is null)
            return Result.Failure(Error.NotFound("ProductAttribute.NotFound", "Attribute not found."));

        productAttributeRepository.Remove(attribute);
        auditLogService.Log("ProductAttribute", attribute.Id.ToString(), "Deleted", $"Deleted attribute '{attribute.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
