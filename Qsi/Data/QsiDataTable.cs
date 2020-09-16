using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data
{
    public sealed class QsiDataTable
    {
        public QsiDataTableType Type { get; set; }

        public QsiQualifiedIdentifier Identifier { get; set; }

        public bool HasIdentifier => Identifier != null;

        public bool IsSystem { get; set; }

        public IReadOnlyList<QsiDataColumn> Columns => _columns;

        internal IEnumerable<QsiDataColumn> VisibleColumns => _columns.Where(c => c.IsVisible);

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
