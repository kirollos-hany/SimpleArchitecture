using SimpleArchitecture.Notifications.Types;

namespace SimpleArchitecture.Notifications.Interfaces;

public interface INotificationPusher
{
    Task<NotificationPushResponse> PushAsync(string deviceToken, string title, string body, IReadOnlyDictionary<string, string> payload, string? webPushUrl = default, string? imageUrl = default, CancellationToken cancellationToken = default);
}

