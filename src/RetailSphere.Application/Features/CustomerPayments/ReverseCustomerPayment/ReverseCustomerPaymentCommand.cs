using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.ReverseCustomerPayment;

public sealed record ReverseCustomerPaymentCommand(long Id, string Reason) : IRequest<Result<CustomerPaymentDto>>;
