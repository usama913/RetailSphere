using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// One selectable value of a <see cref="ProductAttribute"/> (e.g. "42" under the
/// "Size" attribute). A child entity — only ever created/removed through its
/// owning ProductAttribute aggregate, never persisted independently.
/// </summary>
public sealed class AttributeValue : Entity<long>
{
    public long ProductAttributeId { get; private set; }

    public string Value { get; private set; } = default!;

    public int DisplayOrder { get; private set; }

    private AttributeValue()
    {
    }

    internal static AttributeValue Create(long productAttributeId, string value, int displayOrder) => new()
    {
        ProductAttributeId = productAttributeId,
        Value = value.Trim(),
        DisplayOrder = displayOrder,
    };
}
