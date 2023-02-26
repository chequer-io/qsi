using System.Collections.Generic;
using System.Linq;

namespace Qsi.PostgreSql.Extensions;

internal static class EnumerableExtension
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(i => i is not null).Cast<T>();
    }

    public static IEnumerable<T> ConcatWhereNotNull<T>(this IEnumerable<T> source1, IEnumerable<T?> source2)
    {
        return source1.Concat(source2.Where(i => i is not null).Cast<T>());
    }
}
