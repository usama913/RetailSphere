using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.UpdateCustomerCreditLimit;

public sealed class UpdateCustomerCreditLimitCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateCustomerCreditLimitCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCreditLimitCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerDto>(Error.NotFound("Customer.NotFound", "Customer not found."));

        var updateResult = customer.UpdateCreditLimit(request.CreditLimit);
        if (updateResult.IsFailure)
            return Result.Failure<CustomerDto>(updateResult.Error);

        customerRepository.Update(customer);
        auditLogService.Log("Customer", customer.Id.ToString(), "CreditLimitUpdated", $"Set credit limit for '{customer.Name}' to {(request.CreditLimit.HasValue ? request.CreditLimit.Value.ToString("0.00") : "unlimited")}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CustomerMappings.ToDto(customer));
    }
}
