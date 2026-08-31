namespace SokoHub.Domain.Events.Orders;

public sealed record OrderCancelledEvent : DomainEvent
{
    public required string OrderNumber { get; init; }

    public required string Reason { get; init; }
}
