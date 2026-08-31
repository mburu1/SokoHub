namespace SokoHub.Domain.Modules.Catalog;

public sealed class ProductAttributeValue : Entity
{
    private ProductAttributeValue()
    {
    }

    private ProductAttributeValue(Guid id, Guid attributeId, string value)
        : base(id)
    {
        AttributeId = attributeId;
        Value = value;
    }

    public Guid AttributeId { get; private set; }

    public string Value { get; private set; } = string.Empty;

    internal static ProductAttributeValue Create(Guid attributeId, string value) =>
        new(Guid.Empty, attributeId, Ensure.MaxLength(Ensure.NotBlank(value), 80));
}
