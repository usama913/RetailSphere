namespace RetailSphere.Contracts.Finance;

public sealed class ExpenseDto
{
    public required long Id { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public required DateTime ExpenseDate { get; init; }

    public required decimal Amount { get; init; }

    public required string Category { get; init; }

    public string? Description { get; init; }

    public required bool PaidFromCash { get; init; }

    public long? RecordedByUserId { get; init; }

    public string? RecordedByUserName { get; init; }
}

public sealed class CreateExpenseRequest
{
    public required long BranchId { get; init; }

    public required DateTime ExpenseDate { get; init; }

    public required decimal Amount { get; init; }

    public string? Category { get; init; }

    public string? Description { get; init; }

    public bool PaidFromCash { get; init; } = true;
}

public sealed class UpdateExpenseRequest
{
    public required DateTime ExpenseDate { get; init; }

    public required decimal Amount { get; init; }

    public string? Category { get; init; }

    public string? Description { get; init; }

    public bool PaidFromCash { get; init; } = true;
}
