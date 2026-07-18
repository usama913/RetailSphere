using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;
using Unit = RetailSphere.Domain.Catalog.Unit;

namespace RetailSphere.Application.Features.Units.CreateUnit;

public sealed class CreateUnitCommandHandler(
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateUnitCommand, Result<UnitDto>>
{
    public async Task<Result<UnitDto>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        if (await unitRepository.NameExistsAsync(request.Name.Trim(), cancellationToken: cancellationToken))
            return Result.Failure<UnitDto>(Error.Conflict("Unit.DuplicateName", "A unit with this name already exists."));

        var unitResult = Unit.Create(request.Name, request.ShortCode, request.AllowDecimal);
        if (unitResult.IsFailure)
            return Result.Failure<UnitDto>(unitResult.Error);

        var unit = unitResult.Value;
        unitRepository.Add(unit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Unit", unit.Id.ToString(), "Created", $"Created unit '{unit.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(UnitMappings.ToDto(unit));
    }
}
