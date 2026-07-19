using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Notifications;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<NotificationListDto>> GetNotificationsAsync(bool unreadOnly = false, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default);

    Task<ApiResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> MarkNotificationAsReadAsync(long id, CancellationToken cancellationToken = default);
}
