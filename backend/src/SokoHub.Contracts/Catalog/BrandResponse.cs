namespace SokoHub.Contracts.Catalog;

public record BrandResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description);
