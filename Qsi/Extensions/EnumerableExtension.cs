using System;
using System.Collections.Generic;

namespace Qsi.Extensions
{
    internal static class EnumerableExtension
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T find)
        {
            return source.IndexOf(value => Equals(value, find));
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            int index = -1;

            foreach (var value in source)
            {
                index++;

                if (predicate(value))
                    return index;
            }

            return -1;
        }
    }
}
