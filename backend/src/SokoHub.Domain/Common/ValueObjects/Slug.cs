using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SokoHub.Domain.Common.ValueObjects;

public sealed partial record Slug
{
    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Slug Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToLowerInvariant();
        Ensure.MaxLength(normalized, 160);
        Ensure.That(SlugRegex().IsMatch(normalized), "slug", "Slug may contain lowercase letters, digits, and hyphens.");
        return new Slug(normalized);
    }

    public static Slug From(string text)
    {
        var formD = Ensure.NotBlank(text).Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(formD.Length);
        foreach (var ch in formD)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsAsciiLetterOrDigit(ch))
            {
                builder.Append(char.ToLowerInvariant(ch));
            }
            else if (char.IsWhiteSpace(ch) || ch is '-' or '_')
            {
                builder.Append('-');
            }
        }

        var collapsed = HyphenRegex().Replace(builder.ToString(), "-").Trim('-');
        Ensure.That(collapsed.Length > 0, "slug", "Cannot derive a slug from the given text.");
        return Parse(collapsed);
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.Value;

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex SlugRegex();

    [GeneratedRegex("-{2,}", RegexOptions.CultureInvariant)]
    private static partial Regex HyphenRegex();
}
