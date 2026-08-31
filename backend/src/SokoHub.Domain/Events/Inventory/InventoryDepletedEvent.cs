namespace SokoHub.Domain.Events.Inventory;

public sealed record InventoryDepletedEvent : DomainEvent
{
    public required Guid VariantId { get; init; }

    public required Guid WarehouseId { get; init; }
}
