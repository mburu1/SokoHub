using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SokoHub.Workers;

public class WorkerHost : BackgroundService
{
    private readonly ILogger<WorkerHost> _logger;
    private readonly IServiceProvider _serviceProvider;

    public WorkerHost(ILogger<WorkerHost> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SokoHub Worker Host starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("Worker Host heartbeat...");
                // Here we would trigger scheduled jobs from the registry
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Worker Host main loop");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("SokoHub Worker Host shutting down...");
    }
}
