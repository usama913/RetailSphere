using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Units.DeleteUnit;

public sealed record DeleteUnitCommand(long Id) : IRequest<Result>;
