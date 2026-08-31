namespace SokoHub.Domain.Modules.Checkout;

public enum CheckoutStatus
{
    Open = 0,
    InventoryReserved = 1,
    AwaitingPayment = 2,
    Completed = 3,
    Abandoned = 4,
    Failed = 5
}
