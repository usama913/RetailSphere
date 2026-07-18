using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.DeactivateBranch;

public sealed record DeactivateBranchCommand(long Id) : IRequest<Result>;
