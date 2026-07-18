using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.DeactivateCustomer;

public sealed record DeactivateCustomerCommand(long Id) : IRequest<Result>;
