using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.CustomerPayments.Common;

/// <summary>Resolves the CustomerName/BranchName/SalesOrderNumber/ReversedByUserName lookups CustomerPaymentDto needs — mirrors SupplierPaymentDtoAssembler.</summary>
public sealed class CustomerPaymentDtoAssembler(
    ICustomerRepository customerRepository,
    IBranchRepository branchRepository,
    ISalesOrderRepository salesOrderRepository,
    IUserRepository userRepository)
{
    public async Task<CustomerPaymentDto> ToDtoAsync(CustomerPayment payment, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(payment.CustomerId, cancellationToken);
        var branch = await branchRepository.GetByIdAsync(payment.BranchId, cancellationToken);
        var reversedBy = payment.ReversedByUserId.HasValue ? await userRepository.GetByIdAsync(payment.ReversedByUserId.Value, cancellationToken) : null;

        var allocations = new List<CustomerPaymentAllocationDto>();
        foreach (var allocation in payment.Allocations)
        {
            var order = await salesOrderRepository.GetByIdAsync(allocation.SalesOrderId, cancellationToken);
            allocations.Add(new CustomerPaymentAllocationDto
            {
                Id = allocation.Id,
                SalesOrderId = allocation.SalesOrderId,
                SalesOrderNumber = order?.OrderNumber,
                Amount = allocation.Amount,
            });
        }

        return CustomerPaymentMappings.ToDto(payment, customer?.Name, branch?.Name, FormatUserName(reversedBy), allocations);
    }

    public async Task<IReadOnlyList<CustomerPaymentDto>> ToDtosAsync(IEnumerable<CustomerPayment> payments, CancellationToken cancellationToken = default)
    {
        var items = payments.ToList();

        var customers = (await customerRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        var reversedByUserIds = items.Where(p => p.ReversedByUserId.HasValue).Select(p => p.ReversedByUserId!.Value).Distinct().ToList();
        var userNames = new Dictionary<long, string>();
        foreach (var userId in reversedByUserIds)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
                userNames[userId] = FormatUserName(user)!;
        }

        var orderIds = items.SelectMany(p => p.Allocations).Select(a => a.SalesOrderId).Distinct().ToList();
        var orderNumbers = new Dictionary<long, string>();
        foreach (var orderId in orderIds)
        {
            var order = await salesOrderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order is not null)
                orderNumbers[orderId] = order.OrderNumber;
        }

        var results = new List<CustomerPaymentDto>();
        foreach (var payment in items)
        {
            var allocations = payment.Allocations
                .Select(a => new CustomerPaymentAllocationDto
                {
                    Id = a.Id,
                    SalesOrderId = a.SalesOrderId,
                    SalesOrderNumber = orderNumbers.TryGetValue(a.SalesOrderId, out var number) ? number : null,
                    Amount = a.Amount,
                })
                .ToList();

            results.Add(CustomerPaymentMappings.ToDto(
                payment,
                customers.TryGetValue(payment.CustomerId, out var customerName) ? customerName : null,
                branches.TryGetValue(payment.BranchId, out var branchName) ? branchName : null,
                payment.ReversedByUserId.HasValue && userNames.TryGetValue(payment.ReversedByUserId.Value, out var userName) ? userName : null,
                allocations));
        }

        return results;
    }

    private static string? FormatUserName(User? user) => user is null ? null : $"{user.FirstName} {user.LastName}";
}
