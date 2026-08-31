using System.Globalization;

namespace SokoHub.Domain.Common.ValueObjects;

public sealed record OrderNumber
{
    public string Value { get; }

    private OrderNumber(string value) => Value = value;

    public static OrderNumber Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToUpperInvariant();
        Ensure.That(normalized.StartsWith("SH-", StringComparison.Ordinal), "order_number", "Order number must start with SH-.");
        return new OrderNumber(normalized);
    }

    public static OrderNumber Next(DateTimeOffset occurredAt, int sequence)
    {
        Ensure.Positive(sequence);
        var stamp = occurredAt.UtcDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        return new OrderNumber($"SH-{stamp}-{sequence:D6}");
    }

    public override string ToString() => Value;

    public static implicit operator string(OrderNumber number) => number.Value;
}
