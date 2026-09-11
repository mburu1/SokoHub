namespace SokoHub.Notifications;

/// <summary>
/// Abstraction for dispatching notifications across channels (email, SMS, push).
/// </summary>
public interface INotificationService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSmsAsync(string phone, string message);
    Task SendPushNotificationAsync(Guid userId, string title, string body);
}
