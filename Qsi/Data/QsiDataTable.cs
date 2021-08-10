using System;
using Qsi.Data.Cache;

namespace Qsi.Data
{
    public sealed class QsiDataTable
    {
        public QsiTableStructure Table { get; }

        public QsiDataRowCollection Rows { get; }

        public QsiDataTable(QsiTableStructure table, Func<IQsiDataTableCacheProvider> cacheProviderFactory)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Rows = new QsiDataRowCollection(table.Columns.Count, cacheProviderFactory());
        }
    }
}
