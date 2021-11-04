using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Qsi.Data;
using Qsi.Data.Object;

namespace Qsi.Utilities
{
    public static class QsiUtility
    {
        public static IEnumerable<QsiTableStructure> FlattenTables(QsiTableStructure structure)
        {
            return FlattenCore(structure, c => c.References, _ => true);
        }

        public static IEnumerable<QsiTableColumn> FlattenColumns(QsiTableColumn column)
        {
            return FlattenCore(column, c => c.References, _ => true);
        }

        public static IEnumerable<QsiTableStructure> FlattenReferenceTables(QsiTableStructure structure)
        {
            return FlattenCore(structure, c => c.References, x => IsReferenceType(x.Type));
        }

        public static IEnumerable<QsiTableColumn> FlattenReferenceColumns(QsiTableColumn column)
        {
            return FlattenCore(column, c => c.References, x => IsReferenceType(x.Parent.Type));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<T> FlattenCore<T>(T source, Func<T, IEnumerable<T>> selector, Predicate<T> predicate)
        {
            return FlattenInternal(source, selector, predicate, new HashSet<T>());

            static IEnumerable<T> FlattenInternal(T source, Func<T, IEnumerable<T>> selector, Predicate<T> predicate, HashSet<T> visited)
            {
                if (predicate(source))
                    yield return source;

                foreach (var child in selector(source).Where(x => !visited.Contains(x)))
                {
                    visited.Add(child);

                    foreach (var childReference in FlattenInternal(child, selector, predicate, visited))
                        yield return childReference;
                }
            }
        }

        public static bool IsReferenceType(QsiTableType type)
        {
            return type
                is QsiTableType.Table
                or QsiTableType.View
                or QsiTableType.MaterializedView
                or QsiTableType.Prepared;
        }
    }
}
