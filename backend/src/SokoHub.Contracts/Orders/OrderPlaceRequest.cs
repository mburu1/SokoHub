using SokoHub.Contracts.Common;

namespace SokoHub.Contracts.Orders;

public record OrderPlaceRequest(
    Guid CustomerId,
    OrderLineRequest[] Items,
    AddressDto ShippingAddress,
    decimal ShippingTotal,
    decimal DiscountTotal,
    string? CouponCode = null,
    string? CustomerNotes = null);
