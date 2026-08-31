namespace SokoHub.Domain.Common.DomainEvents;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    public required Guid AggregateId { get; init; }
}
