using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.GetUnits;

public sealed record GetUnitsQuery(bool IncludeInactive) : IRequest<Result<IReadOnlyList<UnitDto>>>;
