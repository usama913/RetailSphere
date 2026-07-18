using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.ActivateBranch;

public sealed record ActivateBranchCommand(long Id) : IRequest<Result>;
