using Microsoft.Extensions.DependencyInjection;

namespace SokoHub.Reporting;

/// <summary>
/// Registers all SokoHub.Reporting services into the DI container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddReporting(this IServiceCollection services)
    {
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<SalesReportService>();
        services.AddScoped<VendorReportService>();
        services.AddScoped<PlatformReportService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<AnalyticsService>();

        return services;
    }
}
