using System.Linq;

namespace Qsi.Shared.Utilities;

internal static class StringUtility
{
    public static string JoinNotNullOrEmpty(string delimiter, params string[] values)
    {
        return string.Join(delimiter, values.Where(v => !string.IsNullOrEmpty(v)));
    }
}