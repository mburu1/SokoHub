using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Modules.Promotions;
using SokoHub.Domain.Modules.Tax;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Services;

public interface IPricingService
{
    Money CalculateItemPrice(ProductVariant variant, Coupon? coupon = null);
    Money CalculateOrderTotal(IReadOnlyList<OrderItem> items, Percentage taxRate, Coupon? coupon = null);
}

public class PricingService : IPricingService
{
    public Money CalculateItemPrice(ProductVariant variant, Coupon? coupon = null)
    {
        var basePrice = variant.Price;

        if (coupon != null && coupon.IsApplicableTo(variant))
        {
            return coupon.ApplyDiscount(basePrice);
        }

        return basePrice;
    }

    public Money CalculateOrderTotal(IReadOnlyList<OrderItem> items, Percentage taxRate, Coupon? coupon = null)
    {
        decimal subtotal = 0;

        foreach (var item in items)
        {
            // In a real scenario, we'd fetch the variant and apply potential item-level coupons
            subtotal += item.UnitPrice.Amount * item.Quantity;
        }

        // Apply order-level coupon
        if (coupon != null)
        {
            subtotal -= coupon.CalculateDiscount(subtotal).Amount;
        }

        // Apply tax
        var taxAmount = subtotal * taxRate.Value;

        return new Money(subtotal + taxAmount, "KES"); // Assuming KES for East Africa
    }
}
