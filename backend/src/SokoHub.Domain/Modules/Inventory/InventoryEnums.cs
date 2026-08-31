namespace SokoHub.Domain.Modules.Inventory;

public enum InventoryReservationStatus
{
    Active = 0,
    Consumed = 1,
    Released = 2,
    Expired = 3
}

public enum AdjustmentReason
{
    Stocktake = 0,
    Damage = 1,
    Shrinkage = 2,
    ReturnToStock = 3,
    Correction = 4
}
