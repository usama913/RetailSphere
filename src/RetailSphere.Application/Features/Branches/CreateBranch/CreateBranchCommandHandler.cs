using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.Organization;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.CreateBranch;

public sealed class CreateBranchCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        if (await branchRepository.CodeExistsAsync(request.Code, cancellationToken))
            return Result.Failure<BranchDto>(Error.Conflict("Branch.CodeExists", "A branch with this code already exists."));

        var branchResult = Branch.Create(request.Name, request.Code, request.Address, request.City, null, request.CurrencyCode);
        if (branchResult.IsFailure)
            return Result.Failure<BranchDto>(branchResult.Error);

        var branch = branchResult.Value;
        branchRepository.Add(branch);

        // Save once first so the auto-increment Id exists before it's referenced in the audit entry.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Branch", branch.Id.ToString(), "Created", $"Created branch '{branch.Name}' ({branch.Code}).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(BranchMappings.ToDto(branch));
    }
}
