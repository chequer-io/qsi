using System;
using System.Collections.Generic;

namespace Qsi.Extensions;

internal static class IReadOnlyListExtension
{
    #region FindIndex
    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> match)
        => FindIndex(list, 0, list.Count, match);

    public static int FindIndex<T>(this IReadOnlyList<T> list, int startIndex, Predicate<T> match)
        => FindIndex(list, startIndex, list.Count - startIndex, match);

    public static int FindIndex<T>(this IReadOnlyList<T> list, int startIndex, int count, Predicate<T> match)
    {
        if ((uint)startIndex > (uint)list.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0 || startIndex > list.Count - count)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (match == null)
            throw new ArgumentNullException(nameof(match));

        int endIndex = startIndex + count;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (match!(list[i])) return i;
        }

        return -1;
    }
    #endregion

    #region FindLastIndex
    public static int FindLastIndex<T>(this IReadOnlyList<T> list, Predicate<T> match)
        => FindLastIndex(list, list.Count - 1, list.Count, match);

    public static int FindLastIndex<T>(this IReadOnlyList<T> list, int startIndex, Predicate<T> match)
        => FindLastIndex(list, startIndex, startIndex + 1, match);

    public static int FindLastIndex<T>(this IReadOnlyList<T> list, int startIndex, int count, Predicate<T> match)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        if (list.Count == 0)
        {
            if (startIndex != -1)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
        else if ((uint)startIndex >= (uint)list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (count < 0 || startIndex - count + 1 < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        int endIndex = startIndex - count;

        for (int i = startIndex; i > endIndex; i--)
        {
            if (match(list[i]))
            {
                return i;
            }
        }

        return -1;
    }
    #endregion
}