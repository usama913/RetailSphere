using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;

namespace RetailSphere.Application.Features.CustomerPayments;

internal static class CustomerPaymentMappings
{
    public static CustomerPaymentDto ToDto(
        CustomerPayment payment,
        string? customerName,
        string? branchName,
        string? reversedByUserName,
        IReadOnlyList<CustomerPaymentAllocationDto> allocations) => new()
    {
        Id = payment.Id,
        CustomerId = payment.CustomerId,
        CustomerName = customerName,
        BranchId = payment.BranchId,
        BranchName = branchName,
        PaymentDate = payment.PaymentDate,
        Amount = payment.Amount,
        PaymentMethod = payment.PaymentMethod,
        ReferenceNumber = payment.ReferenceNumber,
        Notes = payment.Notes,
        AllocatedAmount = payment.AllocatedAmount,
        UnallocatedAmount = payment.UnallocatedAmount,
        IsReversed = payment.IsReversed,
        ReversalReason = payment.ReversalReason,
        ReversedAtUtc = payment.ReversedAtUtc,
        ReversedByUserName = reversedByUserName,
        Allocations = allocations,
    };
}
