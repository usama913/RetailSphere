using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Purchasing;

/// <summary>Aggregate root: Supplier — Purchase Orders/Purchase Invoices reference a supplier by Id only.</summary>
public sealed class Supplier : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    /// <summary>Suggested defaults for the payment-terms dropdown — freeform in practice (see Supplier.NormalizePaymentTerms's remarks), same convention as Expense.Categories.</summary>
    public static readonly IReadOnlyList<string> PaymentTermsOptions = ["Cash", "Net 15", "Net 30", "Net 45", "Net 60"];

    public string Name { get; private set; } = default!;

    public string? ContactPerson { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    /// <summary>Tax registration number (e.g. NTN) — optional, used on purchase documents.</summary>
    public string? TaxNumber { get; private set; }

    /// <summary>
    /// Soft cap on how much we can owe this supplier at once before Purchase Invoice
    /// entry should raise a warning — enforced the same way as Customer.CreditLimit
    /// (advisory in the UI, not a hard database constraint). Null means no limit set.
    /// </summary>
    public decimal? CreditLimit { get; private set; }

    /// <summary>Default payment terms applied to new Purchase Invoices for this supplier unless overridden per-invoice (e.g. "Net 30").</summary>
    public string PaymentTerms { get; private set; } = "Cash";

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Supplier()
    {
    }

    public static Result<Supplier> Create(
        string name,
        string? contactPerson,
        string? email,
        string? phone,
        string? address,
        string? taxNumber,
        decimal? creditLimit = null,
        string? paymentTerms = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Supplier>(Error.Validation("Supplier.NameRequired", "Supplier name is required."));

        if (creditLimit.HasValue && creditLimit.Value < 0)
            return Result.Failure<Supplier>(Error.Validation("Supplier.InvalidCreditLimit", "Credit limit cannot be negative."));

        return Result.Success(new Supplier
        {
            Name = name.Trim(),
            ContactPerson = contactPerson,
            Email = email,
            Phone = phone,
            Address = address,
            TaxNumber = taxNumber,
            CreditLimit = creditLimit,
            PaymentTerms = NormalizePaymentTerms(paymentTerms),
            IsActive = true,
        });
    }

    public Result UpdateDetails(
        string name,
        string? contactPerson,
        string? email,
        string? phone,
        string? address,
        string? taxNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Supplier.NameRequired", "Supplier name is required."));

        Name = name.Trim();
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        Address = address;
        TaxNumber = taxNumber;
        return Result.Success();
    }

    /// <summary>Separated from UpdateDetails so a credit-terms change can be its own auditable action (see AuditLogService usage in UpdateSupplierCreditTermsCommandHandler).</summary>
    public Result UpdateCreditTerms(decimal? creditLimit, string? paymentTerms)
    {
        if (creditLimit.HasValue && creditLimit.Value < 0)
            return Result.Failure(Error.Validation("Supplier.InvalidCreditLimit", "Credit limit cannot be negative."));

        CreditLimit = creditLimit;
        PaymentTerms = NormalizePaymentTerms(paymentTerms);
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    /// <summary>Freeform, same rationale as Expense.NormalizeCategory — PaymentTermsOptions is only a suggested-defaults list, not an enforced whitelist.</summary>
    private static string NormalizePaymentTerms(string? paymentTerms) =>
        string.IsNullOrWhiteSpace(paymentTerms) ? "Cash" : paymentTerms.Trim();
}
