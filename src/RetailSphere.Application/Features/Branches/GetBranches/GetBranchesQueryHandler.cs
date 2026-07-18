using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.GetBranches;

public sealed class GetBranchesQueryHandler(IBranchRepository branchRepository)
    : IRequestHandler<GetBranchesQuery, Result<IReadOnlyList<BranchDto>>>
{
    public async Task<Result<IReadOnlyList<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await branchRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        var dtos = branches.Select(BranchMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<BranchDto>>(dtos);
    }
}
