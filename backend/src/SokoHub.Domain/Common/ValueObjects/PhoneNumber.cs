using System.Text.RegularExpressions;

namespace SokoHub.Domain.Common.ValueObjects;

public sealed partial record PhoneNumber
{
    public string E164 { get; }

    public string National { get; }

    public string CountryCode { get; }

    private PhoneNumber(string e164, string national, string countryCode)
    {
        E164 = e164;
        National = national;
        CountryCode = countryCode;
    }

    public static PhoneNumber Parse(string value, string defaultCountryCode = "KE")
    {
        var digits = new string(Ensure.NotBlank(value).Where(char.IsDigit).ToArray());
        Ensure.That(digits.Length is >= 9 and <= 15, "phone", "Phone number is invalid.");

        if (defaultCountryCode == "KE")
        {
            return ParseKenya(digits, value);
        }

        var e164 = digits.StartsWith("00", StringComparison.Ordinal)
            ? "+" + digits[2..]
            : digits.StartsWith('+')
                ? "+" + digits
                : "+" + digits;

        return new PhoneNumber(e164, digits, defaultCountryCode);
    }

    public static PhoneNumber Kenya(string value) => Parse(value, "KE");

    public override string ToString() => E164;

    public static implicit operator string(PhoneNumber phone) => phone.E164;

    private static PhoneNumber ParseKenya(string digits, string original)
    {
        string national;
        if (digits.StartsWith("254", StringComparison.Ordinal) && digits.Length == 12)
        {
            national = "0" + digits[3..];
        }
        else if (digits.StartsWith("0", StringComparison.Ordinal) && digits.Length == 10)
        {
            national = digits;
        }
        else if (digits.Length == 9 && (digits[0] is '7' or '1'))
        {
            national = "0" + digits;
        }
        else
        {
            throw new DomainValidationException("phone", $"Kenyan phone number '{original}' is invalid.");
        }

        Ensure.That(KenyaMobileRegex().IsMatch(national), "phone", "Kenyan mobile number must be 07xx or 01xx.");
        var e164 = "+254" + national[1..];
        return new PhoneNumber(e164, national, "KE");
    }

    [GeneratedRegex(@"^0[17]\d{8}$", RegexOptions.CultureInvariant)]
    private static partial Regex KenyaMobileRegex();
}
