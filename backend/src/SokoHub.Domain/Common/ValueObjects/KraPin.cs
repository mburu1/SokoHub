using System.Text.RegularExpressions;

namespace SokoHub.Domain.Common.ValueObjects;

public sealed partial record KraPin
{
    public string Value { get; }

    private KraPin(string value) => Value = value;

    public static KraPin Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToUpperInvariant();
        Ensure.That(PinRegex().IsMatch(normalized), "kra_pin", "KRA PIN must match A000000000A.");
        return new KraPin(normalized);
    }

    public override string ToString() => Value;

    public static implicit operator string(KraPin pin) => pin.Value;

    [GeneratedRegex(@"^[A-Z]\d{9}[A-Z]$", RegexOptions.CultureInvariant)]
    private static partial Regex PinRegex();
}
