using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.DeactivateBrand;

public sealed class DeactivateBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateBrandCommand, Result>
{
    public async Task<Result> Handle(DeactivateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
            return Result.Failure(Error.NotFound("Brand.NotFound", "Brand not found."));

        brand.Deactivate();
        brandRepository.Update(brand);
        auditLogService.Log("Brand", brand.Id.ToString(), "Deactivated", $"Deactivated brand '{brand.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
