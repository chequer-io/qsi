using System;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class SetColumnsPivot
    {
        public QsiIdentifier[] ColumnNames { get; }

        public int[] AffectedIndices { get; }

        public SetColumnsPivot(QsiIdentifier[] columnNames, int[] affectedIndices)
        {
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));
            
            if (affectedIndices == null)
                throw new ArgumentNullException(nameof(affectedIndices));
            
            if (columnNames.Length < affectedIndices.Length)
                throw new ArgumentException($"{nameof(columnNames)}.Length < {nameof(affectedIndices)}.Length");

            ColumnNames = columnNames;
            AffectedIndices = affectedIndices;
        }
    }
}
