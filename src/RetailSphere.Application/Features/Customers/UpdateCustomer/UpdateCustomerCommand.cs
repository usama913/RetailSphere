using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    long Id,
    string Name,
    string? Phone,
    string? Email,
    string? Address) : IRequest<Result<CustomerDto>>;
