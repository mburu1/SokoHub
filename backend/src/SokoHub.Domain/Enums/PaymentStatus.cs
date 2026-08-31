namespace SokoHub.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Authorized = 1,
    Succeeded = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5,
    Voided = 6
}
