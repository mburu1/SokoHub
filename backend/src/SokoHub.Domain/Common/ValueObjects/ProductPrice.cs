namespace SokoHub.Domain.Common.ValueObjects;

public sealed record ProductPrice
{
    public Money ListPrice { get; }

    public Money? SalePrice { get; }

    public DateTimeOffset? SaleStartsAt { get; }

    public DateTimeOffset? SaleEndsAt { get; }

    public ProductPrice(
        Money listPrice,
        Money? salePrice = null,
        DateTimeOffset? saleStartsAt = null,
        DateTimeOffset? saleEndsAt = null)
    {
        Ensure.That(!listPrice.IsZero, "price", "List price must be greater than zero.");
        if (salePrice is { } sale)
        {
            Ensure.That(sale.Currency == listPrice.Currency, "currency_mismatch", "Sale price currency must match list price.");
            Ensure.That(sale.Amount < listPrice.Amount, "sale_price", "Sale price must be less than list price.");
            if (saleStartsAt is not null && saleEndsAt is not null)
            {
                Ensure.That(saleEndsAt > saleStartsAt, "sale_window", "Sale end must be after sale start.");
            }
        }

        ListPrice = listPrice;
        SalePrice = salePrice;
        SaleStartsAt = saleStartsAt;
        SaleEndsAt = saleEndsAt;
    }

    public Money EffectivePrice(DateTimeOffset at)
    {
        if (SalePrice is null)
        {
            return ListPrice;
        }

        var inWindow =
            (SaleStartsAt is null || at >= SaleStartsAt)
            && (SaleEndsAt is null || at <= SaleEndsAt);

        return inWindow ? SalePrice.Value : ListPrice;
    }
}
