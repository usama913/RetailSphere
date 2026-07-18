using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.ActivateCustomer;

public sealed record ActivateCustomerCommand(long Id) : IRequest<Result>;
