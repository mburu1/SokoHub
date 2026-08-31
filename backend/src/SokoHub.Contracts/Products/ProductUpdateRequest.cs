namespace SokoHub.Contracts.Products;

public record ProductUpdateRequest(
    string Name,
    string Description,
    Guid CategoryId,
    Guid? BrandId = null);
