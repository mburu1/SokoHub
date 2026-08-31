namespace SokoHub.Domain.Modules.Catalog;

public sealed class Category : AggregateRoot
{
    private Category()
    {
    }

    private Category(Guid id, string name, Slug slug, Guid? parentId)
        : base(id)
    {
        Name = name;
        Slug = slug;
        ParentId = parentId;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public Slug Slug { get; private set; } = null!;

    public Guid? ParentId { get; private set; }

    public bool IsActive { get; private set; }

    public static Category Create(string name, Guid? parentId = null, Guid? id = null)
    {
        var trimmed = Ensure.MaxLength(Ensure.NotBlank(name), 120);
        return new Category(id ?? Guid.Empty, trimmed, Slug.From(trimmed), parentId);
    }

    public void Rename(string name)
    {
        Name = Ensure.MaxLength(Ensure.NotBlank(name), 120);
        Slug = Slug.From(Name);
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
