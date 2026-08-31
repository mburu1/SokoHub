using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SokoHub.Domain.Common.ValueObjects;

public sealed partial record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Parse(string value)
    {
        var trimmed = Ensure.NotBlank(value).ToLowerInvariant();
        Ensure.MaxLength(trimmed, 254);

        try
        {
            _ = new MailAddress(trimmed);
        }
        catch (FormatException)
        {
            throw new DomainValidationException("email", "Email address is invalid.");
        }

        if (!EmailRegex().IsMatch(trimmed))
        {
            throw new DomainValidationException("email", "Email address is invalid.");
        }

        return new EmailAddress(trimmed);
    }

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress email) => email.Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();
}
