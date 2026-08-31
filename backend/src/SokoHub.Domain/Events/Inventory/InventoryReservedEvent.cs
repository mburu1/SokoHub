namespace SokoHub.Domain.Events.Inventory;

public sealed record InventoryReservedEvent : DomainEvent
{
    public required Guid VariantId { get; init; }

    public required Guid WarehouseId { get; init; }

    public required int Quantity { get; init; }

    public required Guid ReservationId { get; init; }
}
