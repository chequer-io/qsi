using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataInsertActionNode : IQsiDataInsertActionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableDirectivesNode Directives { get; }

        public IQsiTableAccessNode Target { get; }

        public QsiQualifiedIdentifier[] Partitions { get; }

        public IQsiColumnsDeclarationNode Columns { get; }

        public IQsiRowValueExpressionNode[] Rows { get; }

        public IQsiAssignExpressionNode[] Elements { get; }

        public QsiDataConflictAction ConflictAction { get; }

        public IQsiDataConflictActionNode[] ConflictActions { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Directives, Target, Columns).Concat(Rows).Concat(Elements).Concat(ConflictActions);

        public ImmutableDataInsertActionNode(
            IQsiTreeNode parent,
            IQsiTableDirectivesNode directives,
            IQsiTableAccessNode target,
            QsiQualifiedIdentifier[] partitions,
            IQsiColumnsDeclarationNode columns,
            IQsiRowValueExpressionNode[] rows,
            IQsiAssignExpressionNode[] elements,
            QsiDataConflictAction conflictAction,
            IQsiDataConflictActionNode[] conflictActions)
        {
            Parent = parent;
            Directives = directives;
            Target = target;
            Partitions = partitions;
            Columns = columns;
            Rows = rows;
            Elements = elements;
            ConflictAction = conflictAction;
            ConflictActions = conflictActions;
        }
    }
}
