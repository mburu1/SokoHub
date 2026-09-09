using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SokoHub.Domain.Common.DomainEvents;

namespace SokoHub.Infrastructure.Messaging.RabbitMq;

public class RabbitMqBus : IDomainEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqBus> _logger;

    public RabbitMqBus(IConnection connection, IModel channel, ILogger<RabbitMqBus> logger)
    {
        _connection = connection;
        _channel = channel;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        var eventName = typeof(TEvent).Name;
        _channel.ExchangeDeclare(exchange: eventName, type: ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        _channel.BasicPublish(
            exchange: eventName,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }

    public void Subscribe<TEvent, THandler>() where TEvent : IDomainEvent where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        _channel.ExchangeDeclare(exchange: eventName, type: ExchangeType.Fanout);
        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName, exchange: eventName, routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<TEvent>(message);

            if (@event != null)
            {
                // In a real app, we'd resolve THandler from DI
                _logger.LogInformation("Event {EventName} received and processed.", eventName);
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }
}
