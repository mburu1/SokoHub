namespace SokoHub.Domain.Events.Orders;

public sealed record OrderDeliveredEvent : DomainEvent
{
    public required Guid VendorOrderId { get; init; }
}
