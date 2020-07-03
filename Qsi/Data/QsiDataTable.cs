using System.Collections.Generic;

namespace Qsi.Data
{
    public sealed class QsiDataTable
    {
        public QsiDataTableType Type { get; set; }

        public QsiQualifiedIdentifier Identifier { get; set; }

        public IReadOnlyList<QsiDataColumn> Columns => _columns;

        public bool HasIdentifier => Identifier != null;

        private readonly List<QsiDataColumn> _columns;

        public QsiDataTable()
        {
            _columns = new List<QsiDataColumn>();
        }

        public QsiDataColumn NewColumn()
        {
            var column = new QsiDataColumn(this);
            _columns.Add(column);
            return column;
        }
    }
}
