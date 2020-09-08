using System.Collections;
using System.Collections.Generic;
using java.lang;

namespace Qsi.JSql.Extensions
{
    internal static class IterableExtension
    {
        public static IEnumerable AsEnumerable(this Iterable iterable)
        {
            var iterator = iterable.iterator();

            while (iterator.hasNext())
            {
                yield return iterator.next();
            }
        }

        public static IEnumerable<T> AsEnumerable<T>(this Iterable iterable)
        {
            var iterator = iterable.iterator();

            while (iterator.hasNext())
            {
                yield return (T)iterator.next();
            }
        }
    }
}
