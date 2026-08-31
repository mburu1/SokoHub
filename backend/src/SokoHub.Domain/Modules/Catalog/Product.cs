namespace SokoHub.Domain.Modules.Catalog;

public sealed class Product : AggregateRoot
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];
    private readonly List<ProductAttribute> _attributes = [];

    private Product()
    {
    }

    private Product(
        Guid id,
        Guid vendorId,
        Guid categoryId,
        Guid? brandId,
        string name,
        Slug slug,
        string description)
        : base(id)
    {
        VendorId = vendorId;
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Slug = slug;
        Description = description;
        Status = ProductStatus.Draft;
    }

    public Guid VendorId { get; private set; }

    public Guid CategoryId { get; private set; }

    public Guid? BrandId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Slug Slug { get; private set; } = null!;

    public string Description { get; private set; } = string.Empty;

    public ProductStatus Status { get; private set; }

    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();

    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    public IReadOnlyList<ProductAttribute> Attributes => _attributes.AsReadOnly();

    public static Product Create(
        Guid vendorId,
        Guid categoryId,
        string name,
        string description,
        Guid? brandId = null,
        Guid? id = null)
    {
        var product = new Product(
            id ?? Guid.Empty,
            Ensure.NotEmpty(vendorId),
            Ensure.NotEmpty(categoryId),
            brandId,
            Ensure.MaxLength(Ensure.NotBlank(name), 200),
            Slug.From(name),
            Ensure.MaxLength(Ensure.NotBlank(description), 8000));

        return product;
    }

    public ProductVariant AddVariant(Sku sku, ProductPrice price, int? weightGrams = null)
    {
        Ensure.That(_variants.TrueForAll(v => v.Sku != sku), "duplicate_sku", $"SKU '{sku}' already exists on this product.");
        var variant = ProductVariant.Create(Id, sku, price, weightGrams);
        _variants.Add(variant);
        Touch();
        return variant;
    }

    public ProductImage AddImage(string url, string altText, bool isPrimary = false)
    {
        if (isPrimary)
        {
            foreach (var image in _images)
            {
                image.ClearPrimary();
            }
        }

        var next = ProductImage.Create(Id, url, altText, isPrimary || _images.Count == 0, _images.Count);
        _images.Add(next);
        Touch();
        return next;
    }

    public ProductAttribute DefineAttribute(string name)
    {
        var attribute = ProductAttribute.Create(Id, name);
        Ensure.That(_attributes.TrueForAll(a => !string.Equals(a.Name, attribute.Name, StringComparison.OrdinalIgnoreCase)), "duplicate_attribute", $"Attribute '{name}' already exists.");
        _attributes.Add(attribute);
        Touch();
        return attribute;
    }

    public void Recategorize(Guid categoryId)
    {
        CategoryId = Ensure.NotEmpty(categoryId);
        Touch();
    }

    public void Publish()
    {
        Ensure.That(_variants.Count > 0, "product_no_variants", "A product must have at least one variant before it can be published.");
        Ensure.That(_images.Count > 0, "product_no_images", "A product must have at least one image before it can be published.");
        Status = ProductStatus.Active;
        IncrementVersion();
    }

    public void Deactivate()
    {
        Ensure.That(Status == ProductStatus.Active, "product_not_active", "Only active products can be deactivated.");
        Status = ProductStatus.Inactive;
        IncrementVersion();
    }

    public void Archive()
    {
        Status = ProductStatus.Archived;
        IncrementVersion();
    }
}
