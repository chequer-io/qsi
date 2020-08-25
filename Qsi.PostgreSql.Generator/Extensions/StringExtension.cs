using System.Text.RegularExpressions;

namespace Qsi.PostgreSql.Generator.Extensions
{
    internal static class StringExtension
    {
        public static Regex MakeWildcardPattern(this string value)
        {
            value = Regex.Escape(value)
                .Replace(@"/\*\*/", @"(?:/[^/]*)*/")
                .Replace(@"\*", "[^/]*");

            return new Regex($"^{value}$");
        }
    }
}
