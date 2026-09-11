using Microsoft.Extensions.Logging;

namespace SokoHub.Reporting;

/// <summary>
/// Provides real-time and near-real-time analytics aggregations for the admin dashboard.
/// Combines data from multiple reporting sources into dashboard widgets.
/// </summary>
public class DashboardService
{
    private readonly SalesReportService _salesReport;
    private readonly PlatformReportService _platformReport;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        SalesReportService salesReport,
        PlatformReportService platformReport,
        ILogger<DashboardService> logger)
    {
        _salesReport = salesReport;
        _platformReport = platformReport;
        _logger = logger;
    }

    /// <summary>
    /// Returns a full admin dashboard snapshot for the given date range.
    /// </summary>
    public async Task<DashboardSnapshot> GetSnapshotAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Building dashboard snapshot from {Start} to {End}", start, end);

        var gmvTask = _platformReport.GetGmvAsync(start, end, cancellationToken);
        var feeTask = _platformReport.GetFeeRevenueAsync(start, end, cancellationToken);
        var userGrowthTask = _platformReport.GetUserGrowthAsync(start, end, cancellationToken);
        var funnelTask = _platformReport.GetOrderFunnelAsync(start, end, cancellationToken);
        var periodSummaryTask = _salesReport.GetPeriodSummaryAsync(start, end, cancellationToken);

        await Task.WhenAll(gmvTask, feeTask, userGrowthTask, funnelTask, periodSummaryTask);

        var (newCustomers, newVendors) = userGrowthTask.Result;
        var (totalRevenue, orderCount) = periodSummaryTask.Result;

        return new DashboardSnapshot(
            start,
            end,
            gmvTask.Result,
            feeTask.Result,
            newCustomers,
            newVendors,
            totalRevenue,
            orderCount,
            funnelTask.Result);
    }
}

public record DashboardSnapshot(
    DateTime Start,
    DateTime End,
    decimal Gmv,
    decimal FeeRevenue,
    int NewCustomers,
    int NewVendors,
    decimal TotalRevenue,
    int OrderCount,
    OrderFunnelMetrics Funnel);
