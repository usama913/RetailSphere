using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Branches.CreateBranch;

public sealed record CreateBranchCommand(
    string Name,
    string Code,
    string? Address,
    string? City,
    string CurrencyCode) : IRequest<Result<BranchDto>>;
