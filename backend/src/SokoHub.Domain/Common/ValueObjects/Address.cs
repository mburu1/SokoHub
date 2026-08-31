namespace SokoHub.Domain.Common.ValueObjects;

public sealed record Address
{
    public string Line1 { get; }

    public string? Line2 { get; }

    public string City { get; }

    public string County { get; }

    public string? PostalCode { get; }

    public string CountryCode { get; }

    public Address(
        string line1,
        string city,
        string county,
        string? line2 = null,
        string? postalCode = null,
        string countryCode = "KE")
    {
        Line1 = Ensure.MaxLength(Ensure.NotBlank(line1), 200);
        Line2 = string.IsNullOrWhiteSpace(line2) ? null : Ensure.MaxLength(line2.Trim(), 200);
        City = Ensure.MaxLength(Ensure.NotBlank(city), 100);
        County = Ensure.MaxLength(Ensure.NotBlank(county), 100);
        PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : Ensure.MaxLength(postalCode.Trim(), 16);
        CountryCode = Ensure.NotBlank(countryCode).ToUpperInvariant();
        Ensure.That(CountryCode.Length == 2, "country", "Country must be an ISO 3166-1 alpha-2 code.");
    }

    public override string ToString()
    {
        var line2 = string.IsNullOrWhiteSpace(Line2) ? string.Empty : $", {Line2}";
        var postal = string.IsNullOrWhiteSpace(PostalCode) ? string.Empty : $" {PostalCode}";
        return $"{Line1}{line2}, {City}, {County}{postal}, {CountryCode}";
    }
}
