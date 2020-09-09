using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Utilities
{
    public static class QsiUtility
    {
        public static IEnumerable<QsiDataColumn> FlattenReferenceColumns(QsiDataColumn column)
        {
            return GetAffectedReferencesInternal(column, new HashSet<QsiDataColumn>());
        }

        private static IEnumerable<QsiDataColumn> GetAffectedReferencesInternal(QsiDataColumn column, HashSet<QsiDataColumn> visited)
        {
            foreach (var reference in column.References.Where(r => !visited.Contains(r)))
            {
                visited.Add(reference);

                yield return reference;

                foreach (var childReference in GetAffectedReferencesInternal(reference, visited))
                {
                    yield return childReference;
                }

                visited.Remove(reference);
            }
        }
    }
}
