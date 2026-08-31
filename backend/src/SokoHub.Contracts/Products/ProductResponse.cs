namespace SokoHub.Contracts.Products;

public record ProductResponse(
    Guid Id,
    Guid VendorId,
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string Slug,
    string Description,
    string Status,
    IReadOnlyList<ProductVariantResponse> Variants);
