using System;
using System.Text;

namespace Qsi.Shared;

internal sealed class StringBuilderCache
{
    internal const int MaxBuilderSize = 360;
    private const int DefaultCapacity = 16; // == StringBuilder.DefaultCapacity

    [ThreadStatic]
    private static StringBuilder _cachedInstance;

    public static StringBuilder Acquire(int capacity = DefaultCapacity)
    {
        if (capacity <= MaxBuilderSize)
        {
            var sb = _cachedInstance;

            if (sb != null && capacity <= sb.Capacity)
            {
                _cachedInstance = null;
                sb.Clear();
                return sb;
            }
        }

        return new StringBuilder(capacity);
    }

    public static void Release(StringBuilder sb)
    {
        if (sb.Capacity <= MaxBuilderSize)
        {
            _cachedInstance = sb;
        }
    }

    public static string GetStringAndRelease(StringBuilder sb)
    {
        string result = sb.ToString();
        Release(sb);
        return result;
    }
}
