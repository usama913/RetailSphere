using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.UpdateBranch;

public sealed record UpdateBranchCommand(
    long Id,
    string Name,
    string? Address,
    string? City,
    string CurrencyCode) : IRequest<Result<BranchDto>>;
