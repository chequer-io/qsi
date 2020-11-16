using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data
{
    public sealed class QsiTableStructure
    {
        public QsiTableType Type { get; set; }

        public QsiQualifiedIdentifier Identifier { get; set; }

        public bool HasIdentifier => Identifier != null;

        public bool IsSystem { get; set; }

        public List<QsiTableStructure> References { get; } = new List<QsiTableStructure>();

        public IReadOnlyList<QsiTableColumn> Columns => _columns;

        internal IEnumerable<QsiTableColumn> VisibleColumns => _columns.Where(c => c.IsVisible);

        private readonly List<QsiTableColumn> _columns;

        public QsiTableStructure()
        {
            _columns = new List<QsiTableColumn>();
        }

        public QsiTableColumn NewColumn()
        {
            var column = new QsiTableColumn(this);
            _columns.Add(column);
            return column;
        }
    }
}
