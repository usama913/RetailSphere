using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.GetBranches;

public sealed record GetBranchesQuery(bool IncludeInactive) : IRequest<Result<IReadOnlyList<BranchDto>>>;
