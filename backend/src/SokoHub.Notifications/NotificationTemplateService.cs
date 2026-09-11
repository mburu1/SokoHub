using Microsoft.Extensions.Logging;

namespace SokoHub.Notifications;

/// <summary>
/// Resolves and renders notification templates for different notification types.
/// </summary>
public class NotificationTemplateService
{
    private readonly ILogger<NotificationTemplateService> _logger;

    public NotificationTemplateService(ILogger<NotificationTemplateService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Renders the order confirmation email body.
    /// </summary>
    public string RenderOrderConfirmation(string orderNumber, decimal total, string currency)
    {
        return $"""
            Dear Customer,
            
            Your order #{orderNumber} has been confirmed.
            Total: {currency} {total:N2}
            
            Thank you for shopping with SokoHub!
            """;
    }

    /// <summary>
    /// Renders the order shipped email body.
    /// </summary>
    public string RenderOrderShipped(string orderNumber, string trackingNumber)
    {
        return $"""
            Dear Customer,
            
            Your order #{orderNumber} has been shipped.
            Tracking Number: {trackingNumber}
            
            Thank you for shopping with SokoHub!
            """;
    }

    /// <summary>
    /// Renders the payment confirmation SMS.
    /// </summary>
    public string RenderPaymentConfirmationSms(string orderNumber, decimal amount, string currency)
    {
        return $"SokoHub: Payment of {currency} {amount:N2} received for order #{orderNumber}. Thank you!";
    }

    /// <summary>
    /// Renders the payment failure SMS.
    /// </summary>
    public string RenderPaymentFailedSms(string orderNumber)
    {
        return $"SokoHub: Payment failed for order #{orderNumber}. Please retry or contact support.";
    }

    /// <summary>
    /// Renders a vendor new order push notification body.
    /// </summary>
    public (string Title, string Body) RenderVendorNewOrderPush(string orderNumber)
    {
        return ("New Order Received", $"Order #{orderNumber} has been placed. Please confirm.");
    }

    /// <summary>
    /// Renders a generic welcome email body.
    /// </summary>
    public string RenderWelcomeEmail(string displayName)
    {
        return $"""
            Dear {displayName},
            
            Welcome to SokoHub — East Africa's leading multi-vendor marketplace!
            Start exploring thousands of products from verified vendors.
            
            Regards,
            The SokoHub Team
            """;
    }
}
