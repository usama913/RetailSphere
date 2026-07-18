using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.DeactivateUnit;

public sealed class DeactivateUnitCommandHandler(
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateUnitCommand, Result>
{
    public async Task<Result> Handle(DeactivateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetByIdAsync(request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure(Error.NotFound("Unit.NotFound", "Unit not found."));

        unit.Deactivate();
        unitRepository.Update(unit);
        auditLogService.Log("Unit", unit.Id.ToString(), "Deactivated", $"Deactivated unit '{unit.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
