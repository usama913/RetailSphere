using MediatR;
using RetailSphere.Application.Features.Users.Common;
using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.GetUsers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository, UserDtoAssembler userDtoAssembler)
    : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await userRepository.SearchAsync(
            request.Page, request.PageSize, request.Search, request.BranchId, request.IsActive, cancellationToken);

        var dtos = await userDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<UserDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
