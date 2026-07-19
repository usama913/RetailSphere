using Microsoft.Extensions.Options;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Options;
using RetailSphere.Domain.Notifications;
using RetailSphere.Domain.Purchasing;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.Notifications;

/// <summary>
/// The AP/AR Notifications &amp; Alerts engine — invoked once daily by a Hangfire
/// recurring job (registered in Program.cs, since Hangfire itself lives in the API
/// composition root). Scans every outstanding Purchase Invoice and Sales Order and
/// raises the notifications called for by the spec:
///   - SupplierPaymentDue: a purchase invoice is due within the next few days.
///   - SupplierInvoiceOverdue: a purchase invoice is past its due date.
///   - CustomerPaymentOverdue: a credit/split sales order is past its due date.
///   - InvoiceUnpaidPastDue: either side, once it's been overdue a long time
///     (a louder, Critical-severity escalation of the two above).
/// CustomerCreditLimitExceeded is NOT raised here — it's raised inline, at the
/// moment of the breach, by CreateSalesOrderCommandHandler, since that's the one
/// point in the system that actually knows an override just happened.
///
/// ExistsForEntitySinceAsync (checked per-document, per-type) keeps this idempotent
/// across days — running it twice in the same day, or on consecutive days for a
/// still-overdue invoice, doesn't spam duplicate alerts.
/// </summary>
public sealed class NotificationSweepService(
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    ISalesOrderRepository salesOrderRepository,
    INotificationRepository notificationRepository,
    IEmailSender emailSender,
    IOptions<EmailOptions> emailOptions,
    IUnitOfWork unitOfWork)
{
    private const int UpcomingDueReminderWindowDays = 3;
    private const int CriticalOverdueThresholdDays = 30;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var outstandingInvoices = await purchaseInvoiceRepository.GetOutstandingAsync(supplierId: null, cancellationToken);
        foreach (var invoice in outstandingInvoices)
        {
            var daysToDue = (invoice.DueDate.Date - today).Days;

            if (daysToDue is >= 0 and <= UpcomingDueReminderWindowDays)
            {
                var dueText = daysToDue == 0 ? "today" : $"in {daysToDue} day(s)";
                await RaiseIfNotAlreadyRaisedTodayAsync(
                    "SupplierPaymentDue", "Info",
                    $"Purchase invoice '{invoice.SupplierInvoiceNumber}' ({invoice.OutstandingBalance:0.00} outstanding) is due {dueText}.",
                    "PurchaseInvoice", invoice.Id, today, cancellationToken);
            }

            if (daysToDue < 0)
            {
                await RaiseIfNotAlreadyRaisedTodayAsync(
                    "SupplierInvoiceOverdue", "Warning",
                    $"Purchase invoice '{invoice.SupplierInvoiceNumber}' is {-daysToDue} day(s) overdue ({invoice.OutstandingBalance:0.00} outstanding).",
                    "PurchaseInvoice", invoice.Id, today, cancellationToken);
            }

            if (daysToDue <= -CriticalOverdueThresholdDays)
            {
                await RaiseIfNotAlreadyRaisedTodayAsync(
                    "InvoiceUnpaidPastDue", "Critical",
                    $"Purchase invoice '{invoice.SupplierInvoiceNumber}' has been unpaid for over {CriticalOverdueThresholdDays} days ({invoice.OutstandingBalance:0.00} outstanding).",
                    "PurchaseInvoice", invoice.Id, today, cancellationToken);
            }
        }

        var outstandingOrders = await salesOrderRepository.GetOutstandingAsync(cancellationToken);
        foreach (var order in outstandingOrders)
        {
            var dueDate = order.DueDate ?? order.OrderDate;
            var daysToDue = (dueDate.Date - today).Days;

            if (daysToDue < 0)
            {
                await RaiseIfNotAlreadyRaisedTodayAsync(
                    "CustomerPaymentOverdue", "Warning",
                    $"Sales order '{order.OrderNumber}' is {-daysToDue} day(s) overdue ({order.OutstandingBalance:0.00} outstanding).",
                    "SalesOrder", order.Id, today, cancellationToken);
            }

            if (daysToDue <= -CriticalOverdueThresholdDays)
            {
                await RaiseIfNotAlreadyRaisedTodayAsync(
                    "InvoiceUnpaidPastDue", "Critical",
                    $"Sales order '{order.OrderNumber}' has been unpaid for over {CriticalOverdueThresholdDays} days ({order.OutstandingBalance:0.00} outstanding).",
                    "SalesOrder", order.Id, today, cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await SendPendingEmailsAsync(cancellationToken);
    }

    private async Task RaiseIfNotAlreadyRaisedTodayAsync(
        string type, string severity, string message, string relatedEntityType, long relatedEntityId, DateTime sinceUtc, CancellationToken cancellationToken)
    {
        var alreadyRaisedToday = await notificationRepository.ExistsForEntitySinceAsync(type, relatedEntityType, relatedEntityId, sinceUtc, cancellationToken);
        if (alreadyRaisedToday)
            return;

        notificationRepository.Add(Notification.Create(type, severity, message, relatedEntityType, relatedEntityId, userId: null));
    }

    /// <summary>
    /// v1 email fan-out: everything not yet emailed goes to the flat
    /// EmailOptions.NotificationRecipients list, since the system doesn't yet map
    /// notification types to which users hold which finance permission. A safe,
    /// honest simplification — swap this for a real per-user/per-permission lookup
    /// once that's needed.
    /// </summary>
    private async Task SendPendingEmailsAsync(CancellationToken cancellationToken)
    {
        var settings = emailOptions.Value;
        if (!settings.IsConfigured || string.IsNullOrWhiteSpace(settings.NotificationRecipients))
            return;

        var recipients = settings.NotificationRecipients
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        if (recipients.Count == 0)
            return;

        var pending = await notificationRepository.GetPendingEmailAsync(cancellationToken);
        foreach (var notification in pending)
        {
            var subject = $"RetailSphere [{notification.Severity}] {notification.Type}";
            await emailSender.SendAsync(recipients, subject, notification.Message, cancellationToken);
            notification.MarkEmailSent();
            notificationRepository.Update(notification);
        }

        if (pending.Count > 0)
            await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
