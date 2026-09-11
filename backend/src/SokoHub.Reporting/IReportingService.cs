namespace SokoHub.Reporting;

/// <summary>
/// Abstraction for generating analytical reports from the Reporting (PostgreSQL) read store.
/// </summary>
public interface IReportingService
{
    Task<VendorReport> GenerateVendorReportAsync(Guid vendorId, DateTime start, DateTime end);
    Task<PlatformReport> GeneratePlatformReportAsync(DateTime start, DateTime end);
}
