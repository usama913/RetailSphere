using MediatR;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.GetCustomerPayments;

public sealed class GetCustomerPaymentsQueryHandler(ICustomerPaymentRepository customerPaymentRepository, CustomerPaymentDtoAssembler customerPaymentDtoAssembler)
    : IRequestHandler<GetCustomerPaymentsQuery, Result<PagedResult<CustomerPaymentDto>>>
{
    public async Task<Result<PagedResult<CustomerPaymentDto>>> Handle(GetCustomerPaymentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await customerPaymentRepository.SearchAsync(
            request.Page, request.PageSize, request.CustomerId, request.BranchId, request.FromDate, request.ToDate, cancellationToken);

        var dtos = await customerPaymentDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<CustomerPaymentDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
