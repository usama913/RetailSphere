using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.ActivateBranch;

public sealed class ActivateBranchCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ActivateBranchCommand, Result>
{
    public async Task<Result> Handle(ActivateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (branch is null)
            return Result.Failure(Error.NotFound("Branch.NotFound", "Branch not found."));

        branch.Activate();
        branchRepository.Update(branch);
        auditLogService.Log("Branch", branch.Id.ToString(), "Activated", $"Activated branch '{branch.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
