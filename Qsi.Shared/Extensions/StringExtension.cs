using System;

namespace Qsi.Shared.Extensions;

internal static class StringExtension
{
    public static bool EqualsIgnoreCase(this string a, ReadOnlySpan<char> b)
    {
        return b.Equals(a, StringComparison.OrdinalIgnoreCase);
    }

    public static bool EqualsIgnoreCase(this string a, string b)
    {
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    public static bool ContainsIgnoreCase(this string a, string b)
    {
        return a.Contains(b, StringComparison.OrdinalIgnoreCase);
    }
}