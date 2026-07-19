using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Options;

namespace RetailSphere.Infrastructure.Email;

/// <summary>
/// Plain SMTP sender (works with any SMTP relay, including SendGrid's SMTP
/// endpoint — no vendor SDK needed). When EmailOptions.Host is blank (the
/// out-of-the-box default), SendAsync is a safe no-op that just logs at Information
/// level, so the Notifications engine's email side switches on automatically the
/// moment real credentials are supplied in configuration, with no code changes.
/// </summary>
public sealed class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(IReadOnlyList<string> toAddresses, string subject, string body, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;

        if (!settings.IsConfigured)
        {
            logger.LogInformation(
                "Email not sent (no SMTP host configured) — subject: '{Subject}', intended recipients: {Recipients}",
                subject, string.Join(", ", toAddresses));
            return;
        }

        if (toAddresses.Count == 0)
        {
            logger.LogInformation("Email not sent — no recipients resolved for subject '{Subject}'.", subject);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(settings.FromAddress, settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };

        foreach (var address in toAddresses)
            message.To.Add(address);

        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.EnableSsl,
            Credentials = string.IsNullOrWhiteSpace(settings.Username)
                ? null
                : new NetworkCredential(settings.Username, settings.Password),
        };

        try
        {
            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Notifications are best-effort on the email side — a down/misconfigured
            // SMTP relay should never fail the in-app Notification that triggered it,
            // since that one has already been saved by this point.
            logger.LogWarning(ex, "Failed to send notification email with subject '{Subject}'.", subject);
        }
    }
}
