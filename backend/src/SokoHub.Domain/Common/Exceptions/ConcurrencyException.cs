namespace SokoHub.Domain.Common.Exceptions;

public sealed class ConcurrencyException : DomainException
{
    public ConcurrencyException(string aggregateType, Guid id)
        : base(
            "concurrency_conflict",
            $"{aggregateType} '{id}' was modified by another process.")
    {
        AggregateType = aggregateType;
        AggregateId = id;
    }

    public string AggregateType { get; }

    public Guid AggregateId { get; }
}
