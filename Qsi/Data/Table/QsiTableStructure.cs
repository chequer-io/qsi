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

        public IList<QsiTableStructure> References { get; } = new List<QsiTableStructure>();

        public IList<QsiTableColumn> Columns => _columns;
        
        internal IEnumerable<QsiTableColumn> VisibleColumns => _columns.Where(c => c.IsVisible);

        private readonly QsiTableColumnCollection _columns;

        public QsiTableStructure()
        {
            _columns = new QsiTableColumnCollection(this);
        }

        public QsiTableColumn NewColumn()
        {
            var column = new QsiTableColumn();
            _columns.Add(column);
            
            return column;
        }
    }
}
