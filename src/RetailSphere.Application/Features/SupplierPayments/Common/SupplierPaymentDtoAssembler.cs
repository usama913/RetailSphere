using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.SupplierPayments.Common;

/// <summary>Resolves the SupplierName/InvoiceNumber/BranchName/ReversedByUserName lookups SupplierPaymentDto needs — mirrors ExpenseDtoAssembler.</summary>
public sealed class SupplierPaymentDtoAssembler(
    ISupplierRepository supplierRepository,
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IBranchRepository branchRepository,
    IUserRepository userRepository)
{
    public async Task<SupplierPaymentDto> ToDtoAsync(SupplierPayment payment, CancellationToken cancellationToken = default)
    {
        var supplier = await supplierRepository.GetByIdAsync(payment.SupplierId, cancellationToken);
        var invoice = await purchaseInvoiceRepository.GetByIdAsync(payment.PurchaseInvoiceId, cancellationToken);
        var branch = await branchRepository.GetByIdAsync(payment.BranchId, cancellationToken);
        var reversedBy = payment.ReversedByUserId.HasValue ? await userRepository.GetByIdAsync(payment.ReversedByUserId.Value, cancellationToken) : null;

        return SupplierPaymentMappings.ToDto(payment, supplier?.Name, invoice?.SupplierInvoiceNumber, branch?.Name, FormatUserName(reversedBy));
    }

    public async Task<IReadOnlyList<SupplierPaymentDto>> ToDtosAsync(IEnumerable<SupplierPayment> payments, CancellationToken cancellationToken = default)
    {
        var items = payments.ToList();

        var suppliers = (await supplierRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(s => s.Id, s => s.Name);
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        var invoiceIds = items.Select(p => p.PurchaseInvoiceId).Distinct().ToList();
        var invoiceNumbers = new Dictionary<long, string>();
        foreach (var invoiceId in invoiceIds)
        {
            var invoice = await purchaseInvoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
            if (invoice is not null)
                invoiceNumbers[invoiceId] = invoice.SupplierInvoiceNumber;
        }

        var reversedByUserIds = items.Where(p => p.ReversedByUserId.HasValue).Select(p => p.ReversedByUserId!.Value).Distinct().ToList();
        var userNames = new Dictionary<long, string>();
        foreach (var userId in reversedByUserIds)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
                userNames[userId] = FormatUserName(user)!;
        }

        return items
            .Select(payment => SupplierPaymentMappings.ToDto(
                payment,
                suppliers.TryGetValue(payment.SupplierId, out var supplierName) ? supplierName : null,
                invoiceNumbers.TryGetValue(payment.PurchaseInvoiceId, out var invoiceNumber) ? invoiceNumber : null,
                branches.TryGetValue(payment.BranchId, out var branchName) ? branchName : null,
                payment.ReversedByUserId.HasValue && userNames.TryGetValue(payment.ReversedByUserId.Value, out var userName) ? userName : null))
            .ToList();
    }

    private static string? FormatUserName(User? user) => user is null ? null : $"{user.FirstName} {user.LastName}";
}
