using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.CreateProductAttribute;

public sealed class CreateProductAttributeCommandHandler(
    IProductAttributeRepository productAttributeRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateProductAttributeCommand, Result<ProductAttributeDto>>
{
    public async Task<Result<ProductAttributeDto>> Handle(CreateProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var attributeResult = ProductAttribute.Create(request.Name);
        if (attributeResult.IsFailure)
            return Result.Failure<ProductAttributeDto>(attributeResult.Error);

        var attribute = attributeResult.Value;
        productAttributeRepository.Add(attribute);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("ProductAttribute", attribute.Id.ToString(), "Created", $"Created attribute '{attribute.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ProductAttributeMappings.ToDto(attribute));
    }
}
