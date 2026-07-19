using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.SalesReturns.Common;

/// <summary>Resolves SalesOrderNumber/BranchName/CustomerName/ProcessedByUserName lookups — mirrors SalesOrderDtoAssembler.</summary>
public sealed class SalesReturnDtoAssembler(
    ISalesOrderRepository salesOrderRepository,
    IBranchRepository branchRepository,
    ICustomerRepository customerRepository,
    IUserRepository userRepository)
{
    public async Task<SalesReturnDto> ToDtoAsync(SalesReturn salesReturn, CancellationToken cancellationToken = default)
    {
        var salesOrder = await salesOrderRepository.GetByIdAsync(salesReturn.SalesOrderId, cancellationToken);
        var branch = await branchRepository.GetByIdAsync(salesReturn.BranchId, cancellationToken);
        var customer = salesReturn.CustomerId.HasValue ? await customerRepository.GetByIdAsync(salesReturn.CustomerId.Value, cancellationToken) : null;
        var processedBy = salesReturn.ProcessedByUserId.HasValue ? await userRepository.GetByIdAsync(salesReturn.ProcessedByUserId.Value, cancellationToken) : null;

        return SalesReturnMappings.ToDto(salesReturn, salesOrder?.OrderNumber, branch?.Name, customer?.Name, FormatUserName(processedBy));
    }

    public async Task<IReadOnlyList<SalesReturnDto>> ToDtosAsync(IEnumerable<SalesReturn> salesReturns, CancellationToken cancellationToken = default)
    {
        var items = salesReturns.ToList();

        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);
        var customers = (await customerRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);

        var dtos = new List<SalesReturnDto>();
        foreach (var salesReturn in items)
        {
            var salesOrder = await salesOrderRepository.GetByIdAsync(salesReturn.SalesOrderId, cancellationToken);
            var processedBy = salesReturn.ProcessedByUserId.HasValue ? await userRepository.GetByIdAsync(salesReturn.ProcessedByUserId.Value, cancellationToken) : null;

            dtos.Add(SalesReturnMappings.ToDto(
                salesReturn,
                salesOrder?.OrderNumber,
                branches.TryGetValue(salesReturn.BranchId, out var branchName) ? branchName : null,
                salesReturn.CustomerId.HasValue && customers.TryGetValue(salesReturn.CustomerId.Value, out var customerName) ? customerName : null,
                FormatUserName(processedBy)));
        }

        return dtos;
    }

    private static string? FormatUserName(User? user) => user is null ? null : $"{user.FirstName} {user.LastName}";
}
