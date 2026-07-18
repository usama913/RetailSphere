using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.DeleteUnit;

public sealed class DeleteUnitCommandHandler(
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteUnitCommand, Result>
{
    public async Task<Result> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetByIdAsync(request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure(Error.NotFound("Unit.NotFound", "Unit not found."));

        unitRepository.Remove(unit);
        auditLogService.Log("Unit", unit.Id.ToString(), "Deleted", $"Deleted unit '{unit.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
