using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.GetCustomers;

public sealed record GetCustomersQuery(bool IncludeInactive) : IRequest<Result<IReadOnlyList<CustomerDto>>>;
