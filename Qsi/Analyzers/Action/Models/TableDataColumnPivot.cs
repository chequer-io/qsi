using System;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class TableDataInsertTarget
    {
        public QsiTableStructure Table { get; }

        public int ColumnCount => Table.Columns.Count;

        public TableDataColumnPivot[] ColumnPivots { get; }

        public QsiDataRowCollection InsertRows => _insertRows ??= new QsiDataRowCollection(ColumnCount);

        public QsiDataRowCollection DuplicateRows => _duplicateRows ??= new QsiDataRowCollection(ColumnCount);

        internal QsiDataRowCollection _insertRows;
        internal QsiDataRowCollection _duplicateRows;

        public TableDataInsertTarget(QsiTableStructure table, TableDataColumnPivot[] pivots)
        {
            if (table.Type != QsiTableType.Table)
                throw new ArgumentException(nameof(table));

            if (pivots.Length != table.Columns.Count)
                throw new ArgumentException(nameof(pivots));

            Table = table;
            ColumnPivots = pivots;
        }
    }
}
