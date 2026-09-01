namespace SokoHub.Contracts.Catalog;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    Guid? ParentCategoryId);
