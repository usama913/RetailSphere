using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Finance;

/// <summary>
/// Aggregate root: one open-to-close cash drawer session at a branch. Only stores
/// what a cashier actually records (who opened/closed it, the two physically-counted
/// balances, timestamps, notes) — the "expected" balance and variance are derived,
/// not stored, computed from SalesOrders/Expenses in that time window (see
/// CashRegisterSessionDtoAssembler), so this aggregate never needs to depend on
/// those other modules' repositories.
/// </summary>
public sealed class CashRegisterSession : AggregateRoot<long>, IAuditableEntity
{
    public static readonly IReadOnlyList<string> Statuses = ["Open", "Closed"];

    public long BranchId { get; private set; }

    public long OpenedByUserId { get; private set; }

    public long? ClosedByUserId { get; private set; }

    public string Status { get; private set; } = "Open";

    public decimal OpeningBalance { get; private set; }

    public decimal? ClosingBalance { get; private set; }

    public DateTime OpenedAtUtc { get; private set; }

    public DateTime? ClosedAtUtc { get; private set; }

    public string? OpeningNotes { get; private set; }

    public string? ClosingNotes { get; private set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    private CashRegisterSession()
    {
    }

    public static Result<CashRegisterSession> Open(long branchId, long openedByUserId, decimal openingBalance, string? notes)
    {
        if (openingBalance < 0)
            return Result.Failure<CashRegisterSession>(Error.Validation("CashRegisterSession.InvalidOpeningBalance", "Opening balance cannot be negative."));

        return Result.Success(new CashRegisterSession
        {
            BranchId = branchId,
            OpenedByUserId = openedByUserId,
            Status = "Open",
            OpeningBalance = openingBalance,
            OpenedAtUtc = DateTime.UtcNow,
            OpeningNotes = notes,
        });
    }

    public Result Close(long closedByUserId, decimal closingBalance, string? notes)
    {
        if (Status != "Open")
            return Result.Failure(Error.Conflict("CashRegisterSession.InvalidStatus", "Only an open session can be closed."));

        if (closingBalance < 0)
            return Result.Failure(Error.Validation("CashRegisterSession.InvalidClosingBalance", "Closing balance cannot be negative."));

        Status = "Closed";
        ClosedByUserId = closedByUserId;
        ClosingBalance = closingBalance;
        ClosedAtUtc = DateTime.UtcNow;
        ClosingNotes = notes;
        return Result.Success();
    }
}
