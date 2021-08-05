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
            Rows = new QsiBaseDataRowCollection(table.Columns.Count);
        }
        
        public QsiDataTable(QsiTableStructure table, IQsiDataTableCacheProvider cacheProvider)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Rows = new CachedDataRowCollection(table.Columns.Count, cacheProvider);
        }
    }
}
