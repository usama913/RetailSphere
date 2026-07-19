using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;

namespace RetailSphere.Application.Features.Customers;

internal static class CustomerMappings
{
    public static CustomerDto ToDto(Customer customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Phone = customer.Phone,
        Email = customer.Email,
        Address = customer.Address,
        CreditLimit = customer.CreditLimit,
        IsActive = customer.IsActive,
    };
}
