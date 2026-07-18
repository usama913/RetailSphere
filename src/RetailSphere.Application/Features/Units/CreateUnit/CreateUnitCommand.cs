using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.CreateUnit;

public sealed record CreateUnitCommand(string Name, string ShortCode, bool AllowDecimal) : IRequest<Result<UnitDto>>;
