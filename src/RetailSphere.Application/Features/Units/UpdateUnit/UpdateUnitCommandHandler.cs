using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.UpdateUnit;

public sealed class UpdateUnitCommandHandler(
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateUnitCommand, Result<UnitDto>>
{
    public async Task<Result<UnitDto>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetByIdAsync(request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure<UnitDto>(Error.NotFound("Unit.NotFound", "Unit not found."));

        if (await unitRepository.NameExistsAsync(request.Name.Trim(), request.Id, cancellationToken))
            return Result.Failure<UnitDto>(Error.Conflict("Unit.DuplicateName", "A unit with this name already exists."));

        var updateResult = unit.UpdateDetails(request.Name, request.ShortCode, request.AllowDecimal);
        if (updateResult.IsFailure)
            return Result.Failure<UnitDto>(updateResult.Error);

        unitRepository.Update(unit);
        auditLogService.Log("Unit", unit.Id.ToString(), "Updated", $"Updated unit '{unit.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(UnitMappings.ToDto(unit));
    }
}
