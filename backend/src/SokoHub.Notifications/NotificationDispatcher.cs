using Microsoft.Extensions.Logging;

namespace SokoHub.Notifications;

/// <summary>
/// Dispatches notifications by routing to the correct channel (email, SMS, push)
/// based on notification type and user preferences.
/// </summary>
public class NotificationDispatcher
{
    private readonly INotificationService _notificationService;
    private readonly NotificationTemplateService _templateService;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        INotificationService notificationService,
        NotificationTemplateService templateService,
        ILogger<NotificationDispatcher> logger)
    {
        _notificationService = notificationService;
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Dispatches an order confirmation notification via email and SMS.
    /// </summary>
    public async Task DispatchOrderConfirmedAsync(
        string email,
        string phone,
        string orderNumber,
        decimal total,
        string currency,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching OrderConfirmed notification for order {OrderNumber}", orderNumber);

        var emailBody = _templateService.RenderOrderConfirmation(orderNumber, total, currency);
        var smsBody = _templateService.RenderPaymentConfirmationSms(orderNumber, total, currency);

        await Task.WhenAll(
            _notificationService.SendEmailAsync(email, $"Order #{orderNumber} Confirmed", emailBody),
            _notificationService.SendSmsAsync(phone, smsBody));
    }

    /// <summary>
    /// Dispatches an order shipped notification via email.
    /// </summary>
    public async Task DispatchOrderShippedAsync(
        string email,
        string orderNumber,
        string trackingNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching OrderShipped notification for order {OrderNumber}", orderNumber);

        var emailBody = _templateService.RenderOrderShipped(orderNumber, trackingNumber);
        await _notificationService.SendEmailAsync(email, $"Order #{orderNumber} Shipped", emailBody);
    }

    /// <summary>
    /// Dispatches a payment failed notification via SMS.
    /// </summary>
    public async Task DispatchPaymentFailedAsync(
        string phone,
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching PaymentFailed notification for order {OrderNumber}", orderNumber);

        var sms = _templateService.RenderPaymentFailedSms(orderNumber);
        await _notificationService.SendSmsAsync(phone, sms);
    }

    /// <summary>
    /// Dispatches a new order push notification to a vendor.
    /// </summary>
    public async Task DispatchVendorNewOrderAsync(
        Guid vendorUserId,
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching VendorNewOrder push for order {OrderNumber} to vendor {VendorUserId}", orderNumber, vendorUserId);

        var (title, body) = _templateService.RenderVendorNewOrderPush(orderNumber);
        await _notificationService.SendPushNotificationAsync(vendorUserId, title, body);
    }

    /// <summary>
    /// Dispatches a welcome notification to a newly registered user.
    /// </summary>
    public async Task DispatchWelcomeAsync(
        string email,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching Welcome notification to {Email}", email);

        var body = _templateService.RenderWelcomeEmail(displayName);
        await _notificationService.SendEmailAsync(email, "Welcome to SokoHub!", body);
    }
}
