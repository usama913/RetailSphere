using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.RemoveAttributeValue;

public sealed class RemoveAttributeValueCommandHandler(
    IProductAttributeRepository productAttributeRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<RemoveAttributeValueCommand, Result<ProductAttributeDto>>
{
    public async Task<Result<ProductAttributeDto>> Handle(RemoveAttributeValueCommand request, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByIdAsync(request.ProductAttributeId, cancellationToken);
        if (attribute is null)
            return Result.Failure<ProductAttributeDto>(Error.NotFound("ProductAttribute.NotFound", "Attribute not found."));

        var removeResult = attribute.RemoveValue(request.AttributeValueId);
        if (removeResult.IsFailure)
            return Result.Failure<ProductAttributeDto>(removeResult.Error);

        productAttributeRepository.Update(attribute);
        auditLogService.Log("ProductAttribute", attribute.Id.ToString(), "ValueRemoved", $"Removed a value from attribute '{attribute.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ProductAttributeMappings.ToDto(attribute));
    }
}
