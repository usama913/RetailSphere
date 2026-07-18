using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// A product-level image (URL reference). v1 stores a plain URL rather than
/// running its own upload/blob-storage pipeline — merchandisers paste a link to
/// an already-hosted image; wiring an upload endpoint is a follow-up, not a
/// blocker for the rest of the Catalog module. Variant-level images are the
/// same simplification — deferred; product-level images cover the common case.
/// </summary>
public sealed class ProductImage : Entity<long>
{
    public long ProductId { get; private set; }

    public string Url { get; private set; } = default!;

    public int DisplayOrder { get; private set; }

    private ProductImage()
    {
    }

    internal static ProductImage Create(long productId, string url, int displayOrder) => new()
    {
        ProductId = productId,
        Url = url.Trim(),
        DisplayOrder = displayOrder,
    };
}
