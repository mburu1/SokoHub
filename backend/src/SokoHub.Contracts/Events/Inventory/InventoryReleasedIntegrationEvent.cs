using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Inventory;

public record InventoryReleasedIntegrationEvent(
    Guid OrderId,
    IReadOnlyList<InventoryReservationItemDto> ReleasedItems) : IntegrationEvent;
