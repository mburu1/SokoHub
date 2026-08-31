namespace SokoHub.Domain.Common.ValueObjects;

public sealed record VendorReference
{
    public string Value { get; }

    private VendorReference(string value) => Value = value;

    public static VendorReference Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToUpperInvariant();
        Ensure.MaxLength(normalized, 32);
        return new VendorReference(normalized);
    }

    public static VendorReference Next(int sequence)
    {
        Ensure.Positive(sequence);
        return new VendorReference($"VND-{sequence:D8}");
    }

    public override string ToString() => Value;

    public static implicit operator string(VendorReference reference) => reference.Value;
}
