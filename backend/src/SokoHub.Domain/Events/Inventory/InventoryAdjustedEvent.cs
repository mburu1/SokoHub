namespace SokoHub.Domain.Events.Inventory;

public sealed record InventoryAdjustedEvent : DomainEvent
{
    public required Guid VariantId { get; init; }

    public required int Delta { get; init; }

    public required int OnHand { get; init; }

    public required string Reason { get; init; }
}
