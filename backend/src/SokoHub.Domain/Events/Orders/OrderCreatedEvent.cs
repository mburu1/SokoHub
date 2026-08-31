namespace SokoHub.Domain.Events.Orders;

public sealed record OrderCreatedEvent : DomainEvent
{
    public required Guid CustomerId { get; init; }

    public required string OrderNumber { get; init; }

    public required Money GrandTotal { get; init; }
}
