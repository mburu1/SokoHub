namespace SokoHub.Contracts.Products;

public record ProductCreateRequest(
    Guid VendorId,
    Guid CategoryId,
    string Name,
    string Description,
    Guid? BrandId = null);
