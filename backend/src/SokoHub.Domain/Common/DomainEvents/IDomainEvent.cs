namespace SokoHub.Domain.Common.DomainEvents;

public interface IDomainEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }

    Guid AggregateId { get; }
}
