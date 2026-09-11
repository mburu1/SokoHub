using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Inventory;

public record InventoryAdjustedIntegrationEvent(
    Guid ProductId,
    Guid VariantId,
    int NewQuantityOnHand,
    string Reason) : IntegrationEvent;
