using System;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class DataManipulationTarget
    {
        public QsiTableStructure Table { get; }

        public int ColumnCount => Table.Columns.Count;

        public DataManipulationTargetColumnPivot[] ColumnPivots { get; }

        public QsiDataRowCollection InsertRows => _insertRows ??= new QsiDataRowCollection(ColumnCount);

        public QsiDataRowCollection DuplicateRows => _duplicateRows ??= new QsiDataRowCollection(ColumnCount);

        public QsiDataRowCollection DeleteRows => _deleteRows ??= new QsiDataRowCollection(ColumnCount);

        public QsiDataRowCollection UpdateBeforeRows => _updateBeforeRows ??= new QsiDataRowCollection(ColumnCount);

        public QsiDataRowCollection UpdateAfterRows => _updateAfterRows ??= new QsiDataRowCollection(ColumnCount);

        private QsiDataRowCollection _insertRows;
        private QsiDataRowCollection _duplicateRows;
        private QsiDataRowCollection _deleteRows;
        private QsiDataRowCollection _updateBeforeRows;
        private QsiDataRowCollection _updateAfterRows;

        public DataManipulationTarget(QsiTableStructure table, DataManipulationTargetColumnPivot[] pivots)
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
