using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.GetUsers;

public sealed record GetUsersQuery(
    int Page,
    int PageSize,
    string? Search,
    long? BranchId,
    bool? IsActive) : IRequest<Result<PagedResult<UserDto>>>;
