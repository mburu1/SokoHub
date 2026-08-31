namespace SokoHub.Contracts.Products;

public record ProductVariantRequest(
    string Sku,
    decimal Price,
    int? WeightGrams = null);
