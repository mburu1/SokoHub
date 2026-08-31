namespace SokoHub.Domain.Modules.Catalog;

public sealed class ProductAttribute : Entity
{
    private readonly List<ProductAttributeValue> _values = [];

    private ProductAttribute()
    {
    }

    private ProductAttribute(Guid id, Guid productId, string name)
        : base(id)
    {
        ProductId = productId;
        Name = name;
    }

    public Guid ProductId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyList<ProductAttributeValue> Values => _values.AsReadOnly();

    internal static ProductAttribute Create(Guid productId, string name) =>
        new(Guid.Empty, productId, Ensure.MaxLength(Ensure.NotBlank(name), 80));

    public ProductAttributeValue AddValue(string value)
    {
        var next = ProductAttributeValue.Create(Id, value);
        Ensure.That(_values.TrueForAll(v => !string.Equals(v.Value, next.Value, StringComparison.OrdinalIgnoreCase)), "duplicate_value", $"Value '{value}' already exists.");
        _values.Add(next);
        Touch();
        return next;
    }
}
