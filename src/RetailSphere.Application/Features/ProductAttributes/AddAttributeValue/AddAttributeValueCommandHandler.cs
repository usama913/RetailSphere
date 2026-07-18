using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.AddAttributeValue;

public sealed class AddAttributeValueCommandHandler(
    IProductAttributeRepository productAttributeRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<AddAttributeValueCommand, Result<ProductAttributeDto>>
{
    public async Task<Result<ProductAttributeDto>> Handle(AddAttributeValueCommand request, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByIdAsync(request.ProductAttributeId, cancellationToken);
        if (attribute is null)
            return Result.Failure<ProductAttributeDto>(Error.NotFound("ProductAttribute.NotFound", "Attribute not found."));

        var addResult = attribute.AddValue(request.Value);
        if (addResult.IsFailure)
            return Result.Failure<ProductAttributeDto>(addResult.Error);

        productAttributeRepository.Update(attribute);
        auditLogService.Log("ProductAttribute", attribute.Id.ToString(), "ValueAdded", $"Added value '{request.Value}' to attribute '{attribute.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ProductAttributeMappings.ToDto(attribute));
    }
}
