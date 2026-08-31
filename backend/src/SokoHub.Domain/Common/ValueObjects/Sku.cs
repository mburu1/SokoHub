using System.Text.RegularExpressions;

namespace SokoHub.Domain.Common.ValueObjects;

public sealed partial record Sku
{
    public string Value { get; }

    private Sku(string value) => Value = value;

    public static Sku Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToUpperInvariant();
        Ensure.MaxLength(normalized, 64);
        Ensure.That(SkuRegex().IsMatch(normalized), "sku", "SKU may contain letters, digits, hyphen, and underscore only.");
        return new Sku(normalized);
    }

    public override string ToString() => Value;

    public static implicit operator string(Sku sku) => sku.Value;

    [GeneratedRegex(@"^[A-Z0-9]+(?:[-_][A-Z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex SkuRegex();
}
