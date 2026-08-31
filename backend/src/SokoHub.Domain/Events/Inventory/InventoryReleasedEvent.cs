namespace SokoHub.Domain.Events.Inventory;

public sealed record InventoryReleasedEvent : DomainEvent
{
    public required Guid VariantId { get; init; }

    public required Guid ReservationId { get; init; }

    public required int Quantity { get; init; }
}
