using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.CreateCustomer;

public sealed class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Phone) && await customerRepository.PhoneExistsAsync(request.Phone.Trim(), cancellationToken: cancellationToken))
            return Result.Failure<CustomerDto>(Error.Conflict("Customer.DuplicatePhone", "A customer with this phone number already exists."));

        var customerResult = Customer.Create(request.Name, request.Phone, request.Email, request.Address, request.CreditLimit);
        if (customerResult.IsFailure)
            return Result.Failure<CustomerDto>(customerResult.Error);

        var customer = customerResult.Value;
        customerRepository.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Customer", customer.Id.ToString(), "Created", $"Created customer '{customer.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CustomerMappings.ToDto(customer));
    }
}
