using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Inventory;

public record InventoryReservedIntegrationEvent(
    Guid OrderId,
    IReadOnlyList<InventoryReservationItemDto> ReservedItems) : IntegrationEvent;

public record InventoryReservationItemDto(
    Guid ProductId,
    Guid VariantId,
    int Quantity);
