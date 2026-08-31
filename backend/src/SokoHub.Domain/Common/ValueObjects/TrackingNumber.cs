namespace SokoHub.Domain.Common.ValueObjects;

public sealed record TrackingNumber
{
    public string Value { get; }

    private TrackingNumber(string value) => Value = value;

    public static TrackingNumber Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToUpperInvariant();
        Ensure.MaxLength(normalized, 64);
        return new TrackingNumber(normalized);
    }

    public static TrackingNumber Next(Guid shipmentId) =>
        new($"TRK-{shipmentId.ToString("N")[..12].ToUpperInvariant()}");

    public override string ToString() => Value;

    public static implicit operator string(TrackingNumber number) => number.Value;
}
