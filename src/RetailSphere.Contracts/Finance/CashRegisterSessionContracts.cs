namespace RetailSphere.Contracts.Finance;

public sealed class CashRegisterSessionDto
{
    public required long Id { get; init; }

    public required long BranchId { get; init; }

    public string? BranchName { get; init; }

    public required long OpenedByUserId { get; init; }

    public string? OpenedByUserName { get; init; }

    public long? ClosedByUserId { get; init; }

    public string? ClosedByUserName { get; init; }

    public required string Status { get; init; }

    public required decimal OpeningBalance { get; init; }

    public decimal? ClosingBalance { get; init; }

    public required DateTime OpenedAtUtc { get; init; }

    public DateTime? ClosedAtUtc { get; init; }

    public string? OpeningNotes { get; init; }

    public string? ClosingNotes { get; init; }

    /// <summary>Sum of cash-paid sales since this session opened. Only populated for the "current session" lookup — null in history lists to avoid per-row aggregation.</summary>
    public decimal? TotalCashSales { get; init; }

    /// <summary>Sum of cash-paid expenses since this session opened. Only populated for the "current session" lookup.</summary>
    public decimal? TotalCashExpenses { get; init; }

    /// <summary>OpeningBalance + TotalCashSales - TotalCashExpenses. Only populated for the "current session" lookup.</summary>
    public decimal? CurrentCashBalance { get; init; }
}

public sealed class OpenCashRegisterSessionRequest
{
    public required long BranchId { get; init; }

    public required decimal OpeningBalance { get; init; }

    public string? OpeningNotes { get; init; }
}

public sealed class CloseCashRegisterSessionRequest
{
    public required decimal ClosingBalance { get; init; }

    public string? ClosingNotes { get; init; }
}
