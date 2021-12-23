using System;
using Qsi.Data;
using Qsi.Data.Cache;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class DataManipulationTarget
    {
        public QsiTableStructure Table { get; }

        public int ColumnCount => Table.Columns.Count;

        public DataManipulationTargetDataPivot[] DataPivots { get; }

        public QsiDataRowCollection InsertRows => _insertRows ??= new QsiDataRowCollection(ColumnCount, _cacheProviderFactory());

        public QsiDataRowCollection DuplicateRows => _duplicateRows ??= new QsiDataRowCollection(ColumnCount, _cacheProviderFactory());

        public QsiDataRowCollection DeleteRows => _deleteRows ??= new QsiDataRowCollection(ColumnCount, _cacheProviderFactory());

        public QsiDataRowCollection UpdateBeforeRows => _updateBeforeRows ??= new QsiDataRowCollection(ColumnCount, _cacheProviderFactory());

        public QsiDataRowCollection UpdateAfterRows => _updateAfterRows ??= new QsiDataRowCollection(ColumnCount, _cacheProviderFactory());

        private QsiDataRowCollection _insertRows;
        private QsiDataRowCollection _duplicateRows;
        private QsiDataRowCollection _deleteRows;
        private QsiDataRowCollection _updateBeforeRows;
        private QsiDataRowCollection _updateAfterRows;
        
        public readonly Func<IQsiDataTableCacheProvider> _cacheProviderFactory;

        public DataManipulationTarget(QsiTableStructure table, DataManipulationTargetDataPivot[] pivots, Func<IQsiDataTableCacheProvider> cacheProviderFactory)
        {
            if (table.Type != QsiTableType.Table)
                throw new ArgumentException(nameof(table));

            if (pivots.Length != table.Columns.Count)
                throw new ArgumentException(nameof(pivots));

            _cacheProviderFactory = cacheProviderFactory;

            Table = table;
            DataPivots = pivots;
        }
    }
}
