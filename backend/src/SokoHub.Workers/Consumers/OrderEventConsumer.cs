using MediatR;
using SokoHub.Infrastructure.Messaging.RabbitMq;
using Microsoft.Extensions.Logging;

namespace SokoHub.Workers.Consumers;

public class OrderEventConsumer
{
    private readonly ISender _sender;
    private readonly ILogger<OrderEventConsumer> _logger;

    public OrderEventConsumer(ISender sender, ILogger<OrderEventConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task ConsumeAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing OrderCreated event for Order {OrderId}", @event.OrderId);

        // Trigger corresponding application logic (e.g., Notify Vendor)
        // await _sender.Send(new NotifyVendorOfNewOrderCommand(@event.OrderId), cancellationToken);

        await Task.CompletedTask;
    }
}
