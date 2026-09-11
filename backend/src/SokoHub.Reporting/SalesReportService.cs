using Microsoft.Extensions.Logging;

namespace SokoHub.Reporting;

/// <summary>
/// Provides sales analytics aggregations from the PostgreSQL reporting read store.
/// Metrics include daily/monthly revenue, order volumes, and top-performing products.
/// </summary>
public class SalesReportService
{
    private readonly ILogger<SalesReportService> _logger;

    public SalesReportService(ILogger<SalesReportService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns aggregated sales data per day for the given period.
    /// </summary>
    public async Task<IReadOnlyList<DailySalesSummary>> GetDailySalesAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Querying daily sales from {Start} to {End}", start, end);
        // In production: run parameterized SQL against the PostgreSQL reporting DB.
        return Array.Empty<DailySalesSummary>();
    }

    /// <summary>
    /// Returns the top N products by revenue for the given period.
    /// </summary>
    public async Task<IReadOnlyList<TopProductSummary>> GetTopProductsAsync(
        DateTime start,
        DateTime end,
        int top = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Querying top {Top} products from {Start} to {End}", top, start, end);
        return Array.Empty<TopProductSummary>();
    }

    /// <summary>
    /// Returns the total revenue and order count for the given period.
    /// </summary>
    public async Task<(decimal TotalRevenue, int OrderCount)> GetPeriodSummaryAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Querying period summary from {Start} to {End}", start, end);
        return (0m, 0);
    }
}

public record DailySalesSummary(DateTime Date, decimal Revenue, int OrderCount, string Currency = "KES");
public record TopProductSummary(Guid ProductId, string ProductName, decimal Revenue, int UnitsSold);
