using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.GetCustomers;

public sealed class GetCustomersQueryHandler(ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerDto>>>
{
    public async Task<Result<IReadOnlyList<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        var dtos = customers.Select(CustomerMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<CustomerDto>>(dtos);
    }
}
