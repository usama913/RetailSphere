using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Finance;

/// <summary>
/// Aggregate root: a recorded business expense at a branch (rent, utilities,
/// supplies, etc.) — deliberately simple (no approval workflow, no recurring
/// schedules) for a first pass; exists mainly to feed the "Today's Expenses"
/// dashboard widget and the cash register's expected-balance calculation.
/// </summary>
public sealed class Expense : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public static readonly IReadOnlyList<string> Categories = ["Rent", "Utilities", "Salaries", "Supplies", "Maintenance", "Marketing", "Other"];

    public long BranchId { get; private set; }

    public DateTime ExpenseDate { get; private set; }

    public decimal Amount { get; private set; }

    public string Category { get; private set; } = "Other";

    public string? Description { get; private set; }

    /// <summary>True when this expense was paid out of the branch's cash drawer (affects the Cash Register's expected balance); false for bank/other payment.</summary>
    public bool PaidFromCash { get; private set; } = true;

    public long? RecordedByUserId { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    private Expense()
    {
    }

    public static Result<Expense> Create(
        long branchId,
        DateTime expenseDate,
        decimal amount,
        string? category,
        string? description,
        bool paidFromCash,
        long? recordedByUserId)
    {
        if (amount <= 0)
            return Result.Failure<Expense>(Error.Validation("Expense.InvalidAmount", "Amount must be greater than zero."));

        return Result.Success(new Expense
        {
            BranchId = branchId,
            ExpenseDate = expenseDate,
            Amount = amount,
            Category = NormalizeCategory(category),
            Description = description,
            PaidFromCash = paidFromCash,
            RecordedByUserId = recordedByUserId,
        });
    }

    public Result UpdateDetails(DateTime expenseDate, decimal amount, string? category, string? description, bool paidFromCash)
    {
        if (amount <= 0)
            return Result.Failure(Error.Validation("Expense.InvalidAmount", "Amount must be greater than zero."));

        ExpenseDate = expenseDate;
        Amount = amount;
        Category = NormalizeCategory(category);
        Description = description;
        PaidFromCash = paidFromCash;
        return Result.Success();
    }

    /// <summary>
    /// Categories are freeform — any branch can record a brand-new category name
    /// (e.g. "Delivery Fees") — the Categories list above is only a set of
    /// suggested defaults for the UI's dropdown, not an enforced whitelist.
    /// Anything non-blank is trusted as-is (trimmed); only a null/blank category
    /// falls back to "Other".
    /// </summary>
    private static string NormalizeCategory(string? category) =>
        string.IsNullOrWhiteSpace(category) ? "Other" : category.Trim();
}
