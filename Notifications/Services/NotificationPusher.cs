using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using SimpleArchitecture.Notifications.Interfaces;
using SimpleArchitecture.Notifications.Types;

namespace SimpleArchitecture.Notifications.Services;

public class NotificationPusher : INotificationPusher
{
    private readonly FirebaseMessaging _firebaseMessaging;

    private readonly ILogger<NotificationPusher> _logger;

    public NotificationPusher(FirebaseApp firebaseApp, ILogger<NotificationPusher> logger)
    {
        _logger = logger;
        _firebaseMessaging = FirebaseMessaging.GetMessaging(firebaseApp);
    }
    
    public async Task<NotificationPushResponse> PushAsync(string deviceToken, string title, string body, IReadOnlyDictionary<string, string> payload, string? webPushUrl = default, string? imageUrl = default, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Notification = new Notification
            {
                Body = body,
                Title = title,
                ImageUrl = imageUrl
            },
            Webpush = new WebpushConfig
            {
                FcmOptions = new WebpushFcmOptions
                {
                    Link = webPushUrl
                }
            },
            Data = payload,
            Token = deviceToken
        };

        try
        {
            var notificationId = await _firebaseMessaging.SendAsync(message, cancellationToken);
            
            return string.IsNullOrEmpty(notificationId) ? new NotificationPushResponse(false, string.Empty) : new NotificationPushResponse(true, notificationId);
        }
        catch (Exception e)
        {
            _logger.LogError("Device token: {deviceToken} caused exception: {exception}", deviceToken, e);
            
            return new NotificationPushResponse(false, string.Empty);
        }
    }
}