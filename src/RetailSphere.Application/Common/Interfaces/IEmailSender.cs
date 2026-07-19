namespace RetailSphere.Application.Common.Interfaces;

/// <summary>
/// Abstraction over "send an email" so Application code (the Notification sweep
/// job) never touches SmtpClient/SendGrid/etc. directly. The Infrastructure
/// implementation is a safe no-op (just logs) when EmailOptions isn't configured,
/// so nothing breaks in an environment without real SMTP credentials yet.
/// </summary>
public interface IEmailSender
{
    Task SendAsync(IReadOnlyList<string> toAddresses, string subject, string body, CancellationToken cancellationToken = default);
}
