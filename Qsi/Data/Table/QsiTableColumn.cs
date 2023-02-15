using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Expression.Models;
using Qsi.Data.Object;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Data
{
    public class QsiTableColumn
    {
        // TODO: Revert setter access modifier to internal after refactoring QsiTableAnalyzer
        public QsiTableStructure Parent { get; set; }

        public QsiIdentifier Name { get; set; }

        public List<QsiTableColumn> References { get; } = new();

        public List<QsiObject> ObjectReferences { get; } = new();

        public QsiExpression Expression { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsBinding { get; set; }

        public bool IsAnonymous => Name == null;

        public bool IsDynamic { get; set; }

        public string Default { get; set; }

        public bool IsExpression
        {
            get => _isExpression || QsiUtility.FlattenColumns(this).Any(r => r._isExpression);
            set => _isExpression = value;
        }

        internal QsiQualifiedIdentifier ImplicitTableWildcardTarget { get; set; }

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
            column.ImplicitTableWildcardTarget = ImplicitTableWildcardTarget;
            column._isExpression = _isExpression;
            column.Expression = Expression; // TODO: .Clone(); (feature/QP-1751)

            return column;
        }

        protected virtual QsiTableColumn Clone()
        {
            return new();
        }
    }
}
