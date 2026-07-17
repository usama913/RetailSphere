namespace RetailSphere.Common;

/// <summary>
/// Testable clock abstraction — Domain/Application code never calls DateTime.UtcNow
/// directly so unit tests can control time deterministically.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
