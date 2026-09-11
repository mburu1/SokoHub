using Microsoft.Extensions.DependencyInjection;

namespace SokoHub.Notifications;

/// <summary>
/// Registers all SokoHub.Notifications services into the DI container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<NotificationTemplateService>();
        services.AddScoped<NotificationDispatcher>();

        return services;
    }
}
