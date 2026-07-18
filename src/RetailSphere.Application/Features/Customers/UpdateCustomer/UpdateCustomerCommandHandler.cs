using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerDto>(Error.NotFound("Customer.NotFound", "Customer not found."));

        if (!string.IsNullOrWhiteSpace(request.Phone)
            && await customerRepository.PhoneExistsAsync(request.Phone.Trim(), request.Id, cancellationToken))
            return Result.Failure<CustomerDto>(Error.Conflict("Customer.DuplicatePhone", "A customer with this phone number already exists."));

        var updateResult = customer.UpdateDetails(request.Name, request.Phone, request.Email, request.Address);
        if (updateResult.IsFailure)
            return Result.Failure<CustomerDto>(updateResult.Error);

        customerRepository.Update(customer);
        auditLogService.Log("Customer", customer.Id.ToString(), "Updated", $"Updated customer '{customer.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CustomerMappings.ToDto(customer));
    }
}
