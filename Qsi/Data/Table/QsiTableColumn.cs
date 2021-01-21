using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Data
{
    public class QsiTableColumn
    {
        public QsiTableStructure Parent { get; internal set; }

        public QsiIdentifier Name { get; set; }

        public List<QsiTableColumn> References { get; } = new();

        public bool IsVisible { get; set; } = true;

        public bool IsBinding { get; set; }

        public bool IsAnonymous => Name == null;

        public bool IsDynamic { get; set; }

        public string Default { get; set; }

        public bool IsExpression
        {
            get => _isExpression || QsiUtility.FlattenReferenceColumns(this).Any(r => r._isExpression);
            set => _isExpression = value;
        }

        internal IQsiColumnNode ColumnNode { get; set; }

        internal bool _isExpression;

        internal QsiTableColumn CloneInternal()
        {
            var column = Clone();

            column.Name = Name;
            column.References.AddRange(References);
            column.IsVisible = IsVisible;
            column.IsBinding = IsBinding;
            column.IsDynamic = IsDynamic;
            column.Default = Default;
            column.ColumnNode = ColumnNode;
            column._isExpression = _isExpression;

            return column;
        }

        protected virtual QsiTableColumn Clone()
        {
            return new();
        }
    }
}
