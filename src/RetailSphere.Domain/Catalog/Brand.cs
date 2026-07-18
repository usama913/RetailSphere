using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>Aggregate root: Brand (§3) — Products reference a brand by Id only.</summary>
public sealed class Brand : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Brand()
    {
    }

    public static Result<Brand> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Brand>(Error.Validation("Brand.NameRequired", "Brand name is required."));

        return Result.Success(new Brand
        {
            Name = name.Trim(),
            Description = description,
            IsActive = true,
        });
    }

    public Result UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Brand.NameRequired", "Brand name is required."));

        Name = name.Trim();
        Description = description;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
