using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.UpdateBrand;

public sealed class UpdateBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateBrandCommand, Result<BrandDto>>
{
    public async Task<Result<BrandDto>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
            return Result.Failure<BrandDto>(Error.NotFound("Brand.NotFound", "Brand not found."));

        if (await brandRepository.NameExistsAsync(request.Name.Trim(), request.Id, cancellationToken))
            return Result.Failure<BrandDto>(Error.Conflict("Brand.DuplicateName", "A brand with this name already exists."));

        var updateResult = brand.UpdateDetails(request.Name, request.Description);
        if (updateResult.IsFailure)
            return Result.Failure<BrandDto>(updateResult.Error);

        brandRepository.Update(brand);
        auditLogService.Log("Brand", brand.Id.ToString(), "Updated", $"Updated brand '{brand.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(BrandMappings.ToDto(brand));
    }
}
