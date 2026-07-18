using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.ActivateBrand;

public sealed class ActivateBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ActivateBrandCommand, Result>
{
    public async Task<Result> Handle(ActivateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
            return Result.Failure(Error.NotFound("Brand.NotFound", "Brand not found."));

        brand.Activate();
        brandRepository.Update(brand);
        auditLogService.Log("Brand", brand.Id.ToString(), "Activated", $"Activated brand '{brand.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
