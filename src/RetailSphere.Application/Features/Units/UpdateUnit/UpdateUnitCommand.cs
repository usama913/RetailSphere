using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.UpdateUnit;

public sealed record UpdateUnitCommand(long Id, string Name, string ShortCode, bool AllowDecimal) : IRequest<Result<UnitDto>>;
