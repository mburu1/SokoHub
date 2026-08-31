using System.Globalization;

namespace SokoHub.Domain.Common.ValueObjects;

public readonly record struct Percentage
{
    public decimal Value { get; }

    public Percentage(decimal value)
    {
        Value = Ensure.InRange(value, 0m, 100m);
    }

    public decimal Fraction => Value / 100m;

    public static Percentage Zero => new(0);

    public static Percentage FromFraction(decimal fraction)
    {
        Ensure.InRange(fraction, 0m, 1m);
        return new Percentage(fraction * 100m);
    }

    public Money Of(Money money) => money.Allocate(this);

    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{Value:0.##}%");
}
