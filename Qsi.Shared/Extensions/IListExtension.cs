using System.Collections.Generic;

namespace Qsi.Shared.Extensions
{
    internal static class IListExtension
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> source)
        {
            if (list is List<T> bclList)
            {
                bclList.AddRange(source);
                return;
            }

            foreach (var element in source)
                list.Add(element);
        }
    }
}
