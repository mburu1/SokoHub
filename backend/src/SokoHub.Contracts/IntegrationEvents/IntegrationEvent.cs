using MediatR;

namespace SokoHub.Contracts.IntegrationEvents;

public abstract record IntegrationEvent(
    Guid Id,
    DateTimeOffset OccurredOnUtc) : INotification
{
    protected IntegrationEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }
}
