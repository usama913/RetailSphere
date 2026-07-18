using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// A user-manageable, reusable attribute type (e.g. "Size", "Color", "Material")
/// that Product Variants select values from (§3: Attributes / AttributeValues).
/// Unlike Permission (fixed reference data), attributes are merchandising-owned
/// and expected to grow as new product lines are added.
/// </summary>
public sealed class ProductAttribute : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    private readonly List<AttributeValue> _values = [];

    public string Name { get; private set; } = default!;

    public IReadOnlyCollection<AttributeValue> Values => _values.AsReadOnly();

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private ProductAttribute()
    {
    }

    public static Result<ProductAttribute> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<ProductAttribute>(Error.Validation("ProductAttribute.NameRequired", "Attribute name is required."));

        return Result.Success(new ProductAttribute { Name = name.Trim() });
    }

    public Result Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("ProductAttribute.NameRequired", "Attribute name is required."));

        Name = name.Trim();
        return Result.Success();
    }

    public Result<AttributeValue> AddValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<AttributeValue>(Error.Validation("ProductAttribute.ValueRequired", "Value cannot be empty."));

        var trimmed = value.Trim();
        if (_values.Any(v => string.Equals(v.Value, trimmed, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure<AttributeValue>(Error.Conflict("ProductAttribute.ValueExists", "This value already exists for the attribute."));

        var attributeValue = AttributeValue.Create(Id, trimmed, _values.Count);
        _values.Add(attributeValue);
        return Result.Success(attributeValue);
    }

    public Result RemoveValue(long valueId)
    {
        var value = _values.FirstOrDefault(v => v.Id == valueId);
        if (value is null)
            return Result.Failure(Error.NotFound("ProductAttribute.ValueNotFound", "Value not found."));

        _values.Remove(value);
        return Result.Success();
    }
}
