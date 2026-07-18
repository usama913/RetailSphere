using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.UpdateProductAttribute;

public sealed class UpdateProductAttributeCommandHandler(
    IProductAttributeRepository productAttributeRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateProductAttributeCommand, Result<ProductAttributeDto>>
{
    public async Task<Result<ProductAttributeDto>> Handle(UpdateProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attribute is null)
            return Result.Failure<ProductAttributeDto>(Error.NotFound("ProductAttribute.NotFound", "Attribute not found."));

        var renameResult = attribute.Rename(request.Name);
        if (renameResult.IsFailure)
            return Result.Failure<ProductAttributeDto>(renameResult.Error);

        productAttributeRepository.Update(attribute);
        auditLogService.Log("ProductAttribute", attribute.Id.ToString(), "Updated", $"Renamed attribute to '{attribute.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ProductAttributeMappings.ToDto(attribute));
    }
}
