using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Catalog;

namespace SokoHub.Domain.Modules.Promotions;

public class Coupon
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public decimal DiscountValue { get; private set; }
    public bool IsPercentage { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsActive { get; private set; }

    private Coupon() { }

    public Coupon(Guid id, string code, decimal discountValue, bool isPercentage, DateTime expiryDate)
    {
        Id = id;
        Code = code;
        DiscountValue = discountValue;
        IsPercentage = isPercentage;
        ExpiryDate = expiryDate;
        IsActive = true;
    }

    public bool IsApplicableTo(ProductVariant variant)
    {
        // Simplified logic: all coupons apply to all variants for now
        return IsActive && ExpiryDate > DateTime.UtcNow;
    }

    public Money ApplyDiscount(Money basePrice)
    {
        var discount = CalculateDiscount(basePrice);
        return basePrice.Subtract(discount);
    }

    public Money CalculateDiscount(decimal amount)
    {
        var discountAmount = IsPercentage
            ? amount * (DiscountValue / 100m)
            : DiscountValue;

        return Money.Kes(discountAmount);
    }

    public Money CalculateDiscount(Money amount)
    {
        return CalculateDiscount(amount.Amount);
    }
}
