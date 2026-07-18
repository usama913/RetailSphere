using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.ActivateUnit;

public sealed record ActivateUnitCommand(long Id) : IRequest<Result>;
