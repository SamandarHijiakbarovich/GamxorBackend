using System.Text.RegularExpressions;

namespace GamxorOila.Application.Common;

/// <summary>O'zbekiston telefon raqamlarini kanonik 12 raqamli ko'rinishga keltiradi (mijozdagi mantiqga mos).</summary>
public static partial class PhoneNumber
{
    [GeneratedRegex(@"\D")]
    private static partial Regex NonDigits();

    public static string Canonical(string phone)
    {
        var digits = NonDigits().Replace(phone ?? string.Empty, string.Empty);
        if (digits.Length == 9) return "998" + digits;
        if (digits.Length >= 12 && digits.StartsWith("998")) return digits[^12..];
        return digits;
    }

    public static bool IsComplete(string phone) => Canonical(phone).Length == 12;

    /// <summary>+998 90 123 45 67 ko'rinishida formatlaydi.</summary>
    public static string Pretty(string phone)
    {
        var c = Canonical(phone);
        if (c.Length != 12) return phone ?? string.Empty;
        return $"+{c[..3]} {c[3..5]} {c[5..8]} {c[8..10]} {c[10..12]}";
    }
}
