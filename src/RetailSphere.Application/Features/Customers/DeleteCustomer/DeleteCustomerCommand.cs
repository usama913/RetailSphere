using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.DeleteCustomer;

public sealed record DeleteCustomerCommand(long Id) : IRequest<Result>;
