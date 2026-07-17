namespace RetailSphere.Application.Common.Options;

/// <summary>Bound from configuration ("Auth" section) in the API's appsettings.json.</summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public int AccessTokenLifetimeMinutes { get; init; } = 15;

    public int RefreshTokenLifetimeDays { get; init; } = 30;
}
