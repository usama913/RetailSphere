namespace RetailSphere.Application.Common.Options;

/// <summary>
/// Bound from configuration ("Email" section) in the API's appsettings.json.
/// Deliberately safe-by-default: leaving Host blank (the out-of-the-box value)
/// makes SmtpEmailSender a silent no-op that only logs, so the AP/AR Notifications
/// engine works immediately with in-app notifications alone — email delivery
/// switches on the moment real SMTP/SendGrid-SMTP-relay credentials are supplied
/// here, with no code changes required.
/// </summary>
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Host { get; init; } = string.Empty;

    public int Port { get; init; } = 587;

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public bool EnableSsl { get; init; } = true;

    public string FromAddress { get; init; } = "no-reply@retailsphere.local";

    public string FromName { get; init; } = "RetailSphere";

    /// <summary>
    /// Comma-separated fallback recipient list for AP/AR alert emails (overdue
    /// invoices, credit-limit breaches, etc.) — a v1 stand-in for resolving each
    /// notification's real per-user audience by permission/role, which the finance
    /// team can wire up later once they decide who should receive what.
    /// </summary>
    public string NotificationRecipients { get; init; } = string.Empty;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host);
}
