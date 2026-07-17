namespace RetailSphere.Contracts.Common;

/// <summary>
/// Standard response envelope for every API endpoint (§6 of the architecture doc) —
/// success and error responses share the same shape so the Blazor client has a
/// single deserialization path.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public T? Data { get; init; }

    public ApiError? Error { get; init; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };

    public static ApiResponse<T> Fail(ApiError error) => new() { Success = false, Error = error };
}

public sealed class ApiError
{
    public required string Code { get; init; }

    public required string Message { get; init; }

    public string? TraceId { get; init; }

    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}
