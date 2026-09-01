namespace SokoHub.Contracts.Catalog;

public record ProductResponse(
    Guid Id,
    Guid VendorId,
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string Slug,
    string Description,
    string Status);
