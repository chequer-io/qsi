using System;

namespace Qsi.Data
{
    public sealed class QsiDataTable
    {
        public QsiTableStructure Table { get; }

        public QsiDataRowCollection Rows { get; }

        public QsiDataTable(QsiTableStructure table)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Rows = new QsiDataRowCollection(table.Columns.Count);
        }
    }
}
