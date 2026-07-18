using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.DeactivateBranch;

public sealed class DeactivateBranchCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateBranchCommand, Result>
{
    public async Task<Result> Handle(DeactivateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (branch is null)
            return Result.Failure(Error.NotFound("Branch.NotFound", "Branch not found."));

        branch.Deactivate();
        branchRepository.Update(branch);
        auditLogService.Log("Branch", branch.Id.ToString(), "Deactivated", $"Deactivated branch '{branch.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
