using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Utilities
{
    public static class QsiUtility
    {
        public static IEnumerable<QsiTableColumn> FlattenReferenceColumns(QsiTableColumn column)
        {
            return FlattenReferenceColumnsInternal(column, new HashSet<QsiTableColumn>());
        }

        private static IEnumerable<QsiTableColumn> FlattenReferenceColumnsInternal(QsiTableColumn column, HashSet<QsiTableColumn> visited)
        {
            foreach (var reference in column.References.Where(r => !visited.Contains(r)))
            {
                visited.Add(reference);

                yield return reference;

                foreach (var childReference in FlattenReferenceColumnsInternal(reference, visited))
                {
                    yield return childReference;
                }

                visited.Remove(reference);
            }
        }
    }
}
