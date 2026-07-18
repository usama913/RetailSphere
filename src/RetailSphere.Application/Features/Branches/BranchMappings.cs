using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Application.Features.Branches;

internal static class BranchMappings
{
    public static BranchDto ToDto(Branch branch) => new()
    {
        Id = branch.Id,
        Name = branch.Name,
        Code = branch.Code,
        Address = branch.Address,
        City = branch.City,
        CurrencyCode = branch.CurrencyCode,
        IsActive = branch.IsActive,
    };
}
