using System;

namespace Qsi.Shared.Extensions
{
    internal static class StringExtension
    {
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string a, string b)
        {
            return a.Contains(b, StringComparison.OrdinalIgnoreCase);
        }
    }
}
