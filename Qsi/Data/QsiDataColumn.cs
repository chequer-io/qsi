using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Data
{
    public sealed class QsiDataColumn
    {
        public QsiDataTable Parent { get; }

        public QsiIdentifier Name { get; set; }

        public List<QsiDataColumn> References { get; } = new List<QsiDataColumn>();

        public bool IsVisible { get; set; } = true;

        public bool IsBinding { get; set; }

        public bool IsAnonymous => Name == null;

        public bool IsExpression
        {
            get => _isExpression || QsiUtility.FlattenReferenceColumns(this).Any(r => r._isExpression);
            set => _isExpression = value;
        }

        internal bool _isExpression;

        internal QsiDataColumn(QsiDataTable parent)
        {
            Parent = parent;
        }

        internal QsiDataColumn(IQsiBindingColumnNode bindingColumn)
        {
            Name = new QsiIdentifier(bindingColumn.Id, false);
            IsBinding = true;
        }
    }
}
