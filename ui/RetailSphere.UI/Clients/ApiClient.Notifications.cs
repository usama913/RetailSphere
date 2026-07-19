using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Notifications;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<NotificationListDto>> GetNotificationsAsync(bool unreadOnly = false, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default) =>
        GetAsync<NotificationListDto>($"notifications?unreadOnly={unreadOnly}&page={page}&pageSize={pageSize}", cancellationToken);

    public Task<ApiResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default) =>
        GetAsync<int>("notifications/unread-count", cancellationToken);

    public Task<ApiResponse<object>> MarkNotificationAsReadAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"notifications/{id}/mark-read", cancellationToken);
}
