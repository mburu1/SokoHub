namespace SokoHub.Domain.Modules.Catalog;

public sealed class Brand : AggregateRoot
{
    private Brand()
    {
    }

    private Brand(Guid id, string name, Slug slug)
        : base(id)
    {
        Name = name;
        Slug = slug;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public Slug Slug { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static Brand Create(string name, Guid? id = null)
    {
        var trimmed = Ensure.MaxLength(Ensure.NotBlank(name), 120);
        return new Brand(id ?? Guid.Empty, trimmed, Slug.From(trimmed));
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
