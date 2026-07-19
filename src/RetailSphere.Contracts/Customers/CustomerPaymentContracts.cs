namespace RetailSphere.Contracts.Customers;

public sealed class CustomerPaymentAllocationDto
{
    public required long Id { get; init; }

    public required long SalesOrderId { get; init; }

    public string? SalesOrderNumber { get; init; }

    public required decimal Amount { get; init; }
}

public sealed class CustomerPaymentDto
{
    public required long Id { get; init; }

    public required long CustomerId { get; init; }

    public string? CustomerName { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public required DateTime PaymentDate { get; init; }

    public required decimal Amount { get; init; }

    public required string PaymentMethod { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Notes { get; init; }

    public required decimal AllocatedAmount { get; init; }

    public required decimal UnallocatedAmount { get; init; }

    public required bool IsReversed { get; init; }

    public string? ReversalReason { get; init; }

    public DateTime? ReversedAtUtc { get; init; }

    public string? ReversedByUserName { get; init; }

    public required IReadOnlyList<CustomerPaymentAllocationDto> Allocations { get; init; }
}

public sealed class CustomerPaymentAllocationRequest
{
    public required long SalesOrderId { get; init; }

    public required decimal Amount { get; init; }
}

public sealed class RecordCustomerPaymentRequest
{
    public required long CustomerId { get; init; }

    public required long BranchId { get; init; }

    public required DateTime PaymentDate { get; init; }

    public required decimal Amount { get; init; }

    public string? PaymentMethod { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Notes { get; init; }

    /// <summary>Optionally apply this payment to one or more outstanding sales orders at the same time it's recorded — leave empty to record it as an unapplied on-account credit for now.</summary>
    public IReadOnlyList<CustomerPaymentAllocationRequest> Allocations { get; init; } = [];
}

public sealed class UpdateCustomerPaymentRequest
{
    public required DateTime PaymentDate { get; init; }

    public required decimal Amount { get; init; }

    public string? PaymentMethod { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Notes { get; init; }
}

public sealed class ReverseCustomerPaymentRequest
{
    public required string Reason { get; init; }
}

public sealed class AllocateCustomerPaymentRequest
{
    public required long SalesOrderId { get; init; }

    public required decimal Amount { get; init; }
}
