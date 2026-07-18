using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.DeactivateUnit;

public sealed record DeactivateUnitCommand(long Id) : IRequest<Result>;
