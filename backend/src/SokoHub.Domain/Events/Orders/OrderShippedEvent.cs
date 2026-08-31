namespace SokoHub.Domain.Events.Orders;

public sealed record OrderShippedEvent : DomainEvent
{
    public required Guid VendorOrderId { get; init; }

    public required string TrackingNumber { get; init; }
}
