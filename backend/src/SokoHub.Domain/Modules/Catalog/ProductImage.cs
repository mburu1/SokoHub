namespace SokoHub.Domain.Modules.Catalog;

public sealed class ProductImage : Entity
{
    private ProductImage()
    {
    }

    private ProductImage(Guid id, Guid productId, string url, string altText, bool isPrimary, int sortOrder)
        : base(id)
    {
        ProductId = productId;
        Url = url;
        AltText = altText;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }

    public Guid ProductId { get; private set; }

    public string Url { get; private set; } = string.Empty;

    public string AltText { get; private set; } = string.Empty;

    public bool IsPrimary { get; private set; }

    public int SortOrder { get; private set; }

    internal static ProductImage Create(Guid productId, string url, string altText, bool isPrimary, int sortOrder) =>
        new(
            Guid.Empty,
            productId,
            Ensure.MaxLength(Ensure.NotBlank(url), 2048),
            Ensure.MaxLength(Ensure.NotBlank(altText), 200),
            isPrimary,
            Ensure.NotNegative(sortOrder));

    internal void ClearPrimary() => IsPrimary = false;
}
