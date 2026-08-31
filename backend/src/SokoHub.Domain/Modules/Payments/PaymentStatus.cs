namespace SokoHub.Domain.Modules.Payments;

public enum PaymentStatus
{
    Pending = 0,
    Initiated = 1,
    Succeeded = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5,
    PartiallyRefunded = 6
}

public enum PaymentMethod
{
    MpesaStk = 0,
    MpesaC2B = 1,
    Card = 2,
    Wallet = 3,
    BankTransfer = 4,
    CashOnDelivery = 5
}
