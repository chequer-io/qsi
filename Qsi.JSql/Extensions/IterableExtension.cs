using System.Collections.Generic;
using java.lang;

namespace Qsi.JSql.Extensions
{
    internal static class IterableExtension
    {
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
