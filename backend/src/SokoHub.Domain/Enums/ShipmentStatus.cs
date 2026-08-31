namespace SokoHub.Domain.Enums;

public enum ShipmentStatus
{
    Pending = 0,
    LabelCreated = 1,
    PickedUp = 2,
    InTransit = 3,
    OutForDelivery = 4,
    Delivered = 5,
    FailedAttempt = 6,
    ReturnedToSender = 7
}
