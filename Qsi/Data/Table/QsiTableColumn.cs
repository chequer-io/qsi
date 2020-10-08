using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Data
{
    public sealed class QsiTableColumn
    {
        public QsiTableStructure Parent { get; }

        public QsiIdentifier Name { get; set; }

        public List<QsiTableColumn> References { get; } = new List<QsiTableColumn>();

        public bool IsVisible { get; set; } = true;

        public bool IsBinding { get; set; }

        public bool IsUnique { get; set; }

        public bool IsAnonymous => Name == null;

        public bool IsExpression
        {
            get => _isExpression || QsiUtility.FlattenReferenceColumns(this).Any(r => r._isExpression);
            set => _isExpression = value;
        }

        internal bool _isExpression;

        internal QsiTableColumn(QsiTableStructure parent)
        {
            Parent = parent;
        }

        internal QsiTableColumn(IQsiBindingColumnNode bindingColumn)
        {
            Name = new QsiIdentifier(bindingColumn.Id, false);
            IsBinding = true;
        }
    }
}
