using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Application.Common.Interfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
