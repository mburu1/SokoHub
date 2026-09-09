using Microsoft.Extensions.Logging;
using SokoHub.Notifications;

namespace SokoHub.Notifications;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending Email to {To}: {Subject} - {Body}", to, subject, body);
        // Implement SMTP or SendGrid here.
        await Task.CompletedTask;
    }

    public async Task SendSmsAsync(string phone, string message)
    {
        _logger.LogInformation("Sending SMS to {Phone}: {Message}", phone, message);
        // Implement Twilio or AfricasTalking here.
        await Task.CompletedTask;
    }

    public async Task SendPushNotificationAsync(Guid userId, string title, string body)
    {
        _logger.LogInformation("Sending Push Notification to {UserId}: {Title} - {Body}", userId, title, body);
        // Implement Firebase (FCM) or OneSignal here.
        await Task.CompletedTask;
    }
}
