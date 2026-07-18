using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.CreateBrand;

public sealed class CreateBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateBrandCommand, Result<BrandDto>>
{
    public async Task<Result<BrandDto>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brandResult = Brand.Create(request.Name, request.Description);
        if (brandResult.IsFailure)
            return Result.Failure<BrandDto>(brandResult.Error);

        var brand = brandResult.Value;
        brandRepository.Add(brand);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Brand", brand.Id.ToString(), "Created", $"Created brand '{brand.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(BrandMappings.ToDto(brand));
    }
}
