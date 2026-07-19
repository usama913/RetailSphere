using MediatR;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.GetCustomerPaymentById;

public sealed class GetCustomerPaymentByIdQueryHandler(ICustomerPaymentRepository customerPaymentRepository, CustomerPaymentDtoAssembler customerPaymentDtoAssembler)
    : IRequestHandler<GetCustomerPaymentByIdQuery, Result<CustomerPaymentDto>>
{
    public async Task<Result<CustomerPaymentDto>> Handle(GetCustomerPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await customerPaymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("CustomerPayment.NotFound", "Customer payment not found."));

        var dto = await customerPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
