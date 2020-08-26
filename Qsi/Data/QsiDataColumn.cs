using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data
{
    public sealed class QsiDataColumn
    {
        public QsiDataTable Parent { get; }

        public QsiIdentifier Name { get; set; }

        public List<QsiDataColumn> References { get; } = new List<QsiDataColumn>();

        public bool IsVisible { get; set; } = true;

        public bool IsAnonymous => Name == null;

        public bool IsExpression
        {
            get => _isExpression || References.Any(r => r.IsExpression);
            set => _isExpression = value;
        }

        internal bool _isExpression;

        internal QsiDataColumn(QsiDataTable parent)
        {
            Parent = parent;
        }
    }
}
