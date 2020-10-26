using System;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class SetColumnsPivot
    {
        public QsiQualifiedIdentifier[] Columns { get; }

        public int[] AffectedIndices { get; }

        public SetColumnsPivot(QsiQualifiedIdentifier[] columns, int[] affectedIndices)
        {
            if (columns == null)
                throw new ArgumentNullException(nameof(columns));
            
            if (affectedIndices == null)
                throw new ArgumentNullException(nameof(affectedIndices));
            
            if (columns.Length < affectedIndices.Length)
                throw new ArgumentException($"{nameof(columns)}.Length < {nameof(affectedIndices)}.Length");

            Columns = columns;
            AffectedIndices = affectedIndices;
        }
    }
}
