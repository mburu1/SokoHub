namespace SokoHub.Contracts.Products;

public record ProductVariantResponse(
    Guid Id,
    string Sku,
    decimal Price,
    int? WeightGrams,
    bool IsActive);
