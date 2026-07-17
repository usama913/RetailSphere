namespace RetailSphere.Contracts.Common;

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Data { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required long TotalCount { get; init; }

    public bool HasNextPage => (long)Page * PageSize < TotalCount;
}

/// <summary>
/// Standard list-query parameters — every paginated endpoint accepts this shape
/// (§6: pagination/filtering/sorting conventions).
/// </summary>
public class QueryOptions
{
    private const int MaxPageSize = 200;
    private int _pageSize = 25;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    /// <summary>e.g. "-createdAtUtc" (leading '-' = descending).</summary>
    public string? Sort { get; set; }

    public string? Search { get; set; }
}
