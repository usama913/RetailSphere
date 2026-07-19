using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.GetCustomerPayments;

public sealed record GetCustomerPaymentsQuery(
    int Page,
    int PageSize,
    long? CustomerId,
    long? BranchId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<Result<PagedResult<CustomerPaymentDto>>>;
