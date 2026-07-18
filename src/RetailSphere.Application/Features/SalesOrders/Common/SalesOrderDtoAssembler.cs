using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.SalesOrders.Common;

/// <summary>Resolves the BranchName/CustomerName/CashierName lookups SalesOrderDto needs on top of SalesOrder's own scalar fields — mirrors PurchaseOrderDtoAssembler.</summary>
public sealed class SalesOrderDtoAssembler(IBranchRepository branchRepository, ICustomerRepository customerRepository, IUserRepository userRepository)
{
    public async Task<SalesOrderDto> ToDtoAsync(SalesOrder salesOrder, CancellationToken cancellationToken = default)
    {
        var branch = await branchRepository.GetByIdAsync(salesOrder.BranchId, cancellationToken);
        var customer = salesOrder.CustomerId.HasValue ? await customerRepository.GetByIdAsync(salesOrder.CustomerId.Value, cancellationToken) : null;
        var cashier = salesOrder.CashierUserId.HasValue ? await userRepository.GetByIdAsync(salesOrder.CashierUserId.Value, cancellationToken) : null;

        return SalesOrderMappings.ToDto(salesOrder, branch?.Name, customer?.Name, FormatCashierName(cashier));
    }

    public async Task<IReadOnlyList<SalesOrderDto>> ToDtosAsync(IEnumerable<SalesOrder> salesOrders, CancellationToken cancellationToken = default)
    {
        var orders = salesOrders.ToList();

        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);
        var customers = (await customerRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);

        var cashierIds = orders.Where(o => o.CashierUserId.HasValue).Select(o => o.CashierUserId!.Value).Distinct().ToList();
        var cashierNames = new Dictionary<long, string>();
        foreach (var cashierId in cashierIds)
        {
            var user = await userRepository.GetByIdAsync(cashierId, cancellationToken);
            if (user is not null)
                cashierNames[cashierId] = FormatCashierName(user)!;
        }

        return orders
            .Select(order => SalesOrderMappings.ToDto(
                order,
                branches.TryGetValue(order.BranchId, out var branchName) ? branchName : null,
                order.CustomerId.HasValue && customers.TryGetValue(order.CustomerId.Value, out var customerName) ? customerName : null,
                order.CashierUserId.HasValue && cashierNames.TryGetValue(order.CashierUserId.Value, out var cashierName) ? cashierName : null))
            .ToList();
    }

    private static string? FormatCashierName(User? user) => user is null ? null : $"{user.FirstName} {user.LastName}";
}
