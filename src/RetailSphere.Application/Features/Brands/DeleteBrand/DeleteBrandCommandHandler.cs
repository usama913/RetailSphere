using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.DeleteBrand;

public sealed class DeleteBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteBrandCommand, Result>
{
    public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
            return Result.Failure(Error.NotFound("Brand.NotFound", "Brand not found."));

        brandRepository.Remove(brand);
        auditLogService.Log("Brand", brand.Id.ToString(), "Deleted", $"Deleted brand '{brand.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
