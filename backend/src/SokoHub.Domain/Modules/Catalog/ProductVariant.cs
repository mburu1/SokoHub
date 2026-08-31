namespace SokoHub.Domain.Modules.Catalog;

public sealed class ProductVariant : Entity
{
    private ProductVariant()
    {
    }

    private ProductVariant(Guid id, Guid productId, Sku sku, ProductPrice price, int? weightGrams)
        : base(id)
    {
        ProductId = productId;
        Sku = sku;
        Price = price;
        WeightGrams = weightGrams;
        IsActive = true;
    }

    public Guid ProductId { get; private set; }

    public Sku Sku { get; private set; } = null!;

    public ProductPrice Price { get; private set; } = null!;

    public int? WeightGrams { get; private set; }

    public bool IsActive { get; private set; }

    internal static ProductVariant Create(Guid productId, Sku sku, ProductPrice price, int? weightGrams)
    {
        if (weightGrams is { } grams)
        {
            Ensure.Positive(grams);
        }

        return new ProductVariant(Guid.Empty, productId, sku, price, weightGrams);
    }

    public void UpdatePrice(ProductPrice price)
    {
        Price = Ensure.NotNull(price);
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
