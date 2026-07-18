using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.GetUnits;

public sealed class GetUnitsQueryHandler(IUnitRepository unitRepository)
    : IRequestHandler<GetUnitsQuery, Result<IReadOnlyList<UnitDto>>>
{
    public async Task<Result<IReadOnlyList<UnitDto>>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var units = await unitRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        var dtos = units.Select(UnitMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<UnitDto>>(dtos);
    }
}
