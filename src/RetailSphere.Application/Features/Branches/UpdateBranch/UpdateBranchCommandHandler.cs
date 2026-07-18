using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.UpdateBranch;

public sealed class UpdateBranchCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateBranchCommand, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (branch is null)
            return Result.Failure<BranchDto>(Error.NotFound("Branch.NotFound", "Branch not found."));

        var updateResult = branch.UpdateDetails(request.Name, request.Address, request.City, request.CurrencyCode);
        if (updateResult.IsFailure)
            return Result.Failure<BranchDto>(updateResult.Error);

        branchRepository.Update(branch);
        auditLogService.Log("Branch", branch.Id.ToString(), "Updated", $"Updated branch '{branch.Name}' ({branch.Code}).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(BranchMappings.ToDto(branch));
    }
}
