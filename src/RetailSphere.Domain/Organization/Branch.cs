using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Organization;

/// <summary>
/// A physical store/branch. Every operational aggregate elsewhere in the system
/// (StockItem, SalesOrder, CashRegisterSession, ...) carries a BranchId — Branch
/// itself stays a lightweight aggregate so it can be referenced everywhere without
/// creating a hub-and-spoke dependency mess.
/// </summary>
public sealed class Branch : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = default!;

    public string Code { get; private set; } = default!;

    public string? Address { get; private set; }

    public string? City { get; private set; }

    /// <summary>FK to Finance.TaxJurisdictions — determines applicable tax rules (§4.3 of the architecture doc).</summary>
    public long? TaxJurisdictionId { get; private set; }

    /// <summary>ISO 4217 currency code this branch trades in. Defaults to PKR per the Pakistan-first launch decision.</summary>
    public string CurrencyCode { get; private set; } = "PKR";

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Branch()
    {
    }

    public static Result<Branch> Create(string name, string code, string? address, string? city, long? taxJurisdictionId, string currencyCode = "PKR")
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Branch>(Error.Validation("Branch.NameRequired", "Branch name is required."));

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Branch>(Error.Validation("Branch.CodeRequired", "Branch code is required."));

        return Result.Success(new Branch
        {
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant(),
            Address = address,
            City = city,
            TaxJurisdictionId = taxJurisdictionId,
            CurrencyCode = currencyCode,
        });
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
