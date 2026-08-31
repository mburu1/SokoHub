using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SokoHub.Domain.Common.Exceptions;

namespace SokoHub.Domain.Common.Guards;

public static class Ensure
{
    public static T NotNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : class
    {
        if (value is null)
        {
            throw new DomainValidationException("required", $"{name} is required.");
        }

        return value;
    }

    public static string NotBlank(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("required", $"{name} is required.");
        }

        return value.Trim();
    }

    public static Guid NotEmpty(
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value == Guid.Empty)
        {
            throw new DomainValidationException("required", $"{name} is required.");
        }

        return value;
    }

    public static int Positive(
        int value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value <= 0)
        {
            throw new DomainValidationException("positive", $"{name} must be greater than zero.");
        }

        return value;
    }

    public static decimal NotNegative(
        decimal value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value < 0)
        {
            throw new DomainValidationException("not_negative", $"{name} cannot be negative.");
        }

        return value;
    }

    public static int NotNegative(
        int value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value < 0)
        {
            throw new DomainValidationException("not_negative", $"{name} cannot be negative.");
        }

        return value;
    }

    public static string MaxLength(
        string value,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLength);

        if (value.Length > maxLength)
        {
            throw new DomainValidationException(
                "max_length",
                $"{name} cannot exceed {maxLength} characters.");
        }

        return value;
    }

    public static T InRange<T>(
        T value,
        T minInclusive,
        T maxInclusive,
        [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(minInclusive) < 0 || value.CompareTo(maxInclusive) > 0)
        {
            throw new DomainValidationException(
                "range",
                $"{name} must be between {minInclusive} and {maxInclusive}.");
        }

        return value;
    }

    public static void That(
        [DoesNotReturnIf(false)] bool condition,
        string code,
        string message)
    {
        if (!condition)
        {
            throw new InvariantViolationException(code, message);
        }
    }
}
