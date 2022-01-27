using System;
using Qsi.Data.Cache;

namespace Qsi.Data
{
    public sealed class QsiDataTable
    {
        public QsiTableStructure Table { get; }

        public QsiDataRowCollection Rows { get; }
        
        public QsiDataTable(QsiTableStructure table)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Rows = new QsiDataRowCollection(table.Columns.Count, new QsiDataTableMemoryCacheProvider());
        }

        public QsiDataTable(QsiTableStructure table, Func<IQsiDataTableCacheProvider> cacheProviderFactory)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Rows = new QsiDataRowCollection(table.Columns.Count, cacheProviderFactory());
        }

        private QsiDataTable(QsiTableStructure table, QsiDataRowCollection rows)
        {
            Table = table;
            Rows = rows;
        }

        public QsiDataTable CloneVisibleOnly()
        {
            var table = new QsiDataTable(Table.CloneVisibleOnly(), Rows);

            return table;
        } 
    }
}
