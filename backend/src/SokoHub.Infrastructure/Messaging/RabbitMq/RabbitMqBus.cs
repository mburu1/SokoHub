using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Infrastructure.Messaging.RabbitMq;

public class RabbitMqBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqBus> _logger;
    private readonly string _exchange;

    public RabbitMqBus(IConnection connection, IConfiguration config, ILogger<RabbitMqBus> logger)
    {
        _connection = connection;
        _logger = logger;
        _channel = _connection.CreateModel();
        _exchange = config["RabbitMQ:Exchange"] ?? "sokohub.events";

        _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        var routingKey = @event.GetType().Name;
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        _channel.BasicPublish(
            exchange: _exchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body);

        _logger.LogDebug("Published integration event {EventType} with routing key {RoutingKey}", typeof(TEvent).Name, routingKey);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
