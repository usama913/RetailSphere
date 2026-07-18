using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.ActivateUnit;

public sealed class ActivateUnitCommandHandler(
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ActivateUnitCommand, Result>
{
    public async Task<Result> Handle(ActivateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetByIdAsync(request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure(Error.NotFound("Unit.NotFound", "Unit not found."));

        unit.Activate();
        unitRepository.Update(unit);
        auditLogService.Log("Unit", unit.Id.ToString(), "Activated", $"Activated unit '{unit.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
