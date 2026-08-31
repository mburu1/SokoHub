namespace SokoHub.Domain.Modules.Orders;

public enum OrderStatus
{
    PendingPayment = 0,
    Confirmed = 1,
    PartiallyShipped = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6
}

public enum VendorOrderStatus
{
    Pending = 0,
    Accepted = 1,
    Packed = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Rejected = 6
}
