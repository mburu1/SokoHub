using System.Globalization;

namespace SokoHub.Domain.Common.ValueObjects;

public readonly record struct Money : IComparable<Money>
{
    public const string DefaultCurrency = "KES";

    public decimal Amount { get; }

    public string Currency { get; }

    public Money(decimal amount, string currency = DefaultCurrency)
    {
        Amount = decimal.Round(Ensure.NotNegative(amount), 2, MidpointRounding.AwayFromZero);
        Currency = NormalizeCurrency(currency);
    }

    public static Money Zero(string currency = DefaultCurrency) => new(0, currency);

    public static Money Kes(decimal amount) => new(amount, DefaultCurrency);

    public static Money Usd(decimal amount) => new(amount, "USD");

    public bool IsZero => Amount == 0;

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        Ensure.That(Amount >= other.Amount, "money_insufficient", "Resulting amount cannot be negative.");
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        Ensure.NotNegative(factor);
        return new Money(Amount * factor, Currency);
    }

    public Money Allocate(Percentage percentage) => new(Amount * percentage.Fraction, Currency);

    public int CompareTo(Money other)
    {
        EnsureSameCurrency(other);
        return Amount.CompareTo(other.Amount);
    }

    public static Money operator +(Money left, Money right) => left.Add(right);

    public static Money operator -(Money left, Money right) => left.Subtract(right);

    public static Money operator *(Money left, decimal right) => left.Multiply(right);

    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;

    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;

    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{Currency} {Amount:0.00}");

    private void EnsureSameCurrency(Money other)
    {
        Ensure.That(
            Currency == other.Currency,
            "currency_mismatch",
            $"Cannot combine {Currency} with {other.Currency}.");
    }

    private static string NormalizeCurrency(string currency)
    {
        var normalized = Ensure.NotBlank(currency).ToUpperInvariant();
        Ensure.That(normalized.Length == 3 && normalized.All(char.IsAsciiLetter), "currency", "Currency must be a 3-letter ISO 4217 code.");
        return normalized;
    }
}
