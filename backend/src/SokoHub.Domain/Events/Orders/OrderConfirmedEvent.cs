namespace SokoHub.Domain.Events.Orders;

public sealed record OrderConfirmedEvent : DomainEvent
{
    public required Guid CustomerId { get; init; }

    public required string OrderNumber { get; init; }
}
