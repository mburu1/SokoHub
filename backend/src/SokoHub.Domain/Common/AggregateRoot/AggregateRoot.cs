namespace SokoHub.Domain.Common.AggregateRoots;

public abstract class AggregateRoot : SokoHub.Domain.Common.Entities.Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public int Version { get; protected set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void IncrementVersion()
    {
        Version++;
        Touch();
    }
}
