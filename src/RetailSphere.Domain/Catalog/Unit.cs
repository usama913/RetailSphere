using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// Aggregate root: Unit of measure (e.g. "Each", "Kilogram", "Box") — Products
/// reference a unit by Id only, the same "plain column, no EF navigation"
/// pattern used for Category/Brand.
/// </summary>
public sealed class Unit : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = default!;

    public string ShortCode { get; private set; } = default!;

    /// <summary>False for count-based units (e.g. "Pieces") where fractional quantities don't make sense.</summary>
    public bool AllowDecimal { get; private set; } = true;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Unit()
    {
    }

    public static Result<Unit> Create(string name, string shortCode, bool allowDecimal)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Unit>(Error.Validation("Unit.NameRequired", "Unit name is required."));

        if (string.IsNullOrWhiteSpace(shortCode))
            return Result.Failure<Unit>(Error.Validation("Unit.ShortCodeRequired", "Unit short code is required."));

        return Result.Success(new Unit
        {
            Name = name.Trim(),
            ShortCode = shortCode.Trim(),
            AllowDecimal = allowDecimal,
            IsActive = true,
        });
    }

    public Result UpdateDetails(string name, string shortCode, bool allowDecimal)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Unit.NameRequired", "Unit name is required."));

        if (string.IsNullOrWhiteSpace(shortCode))
            return Result.Failure(Error.Validation("Unit.ShortCodeRequired", "Unit short code is required."));

        Name = name.Trim();
        ShortCode = shortCode.Trim();
        AllowDecimal = allowDecimal;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
