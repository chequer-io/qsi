using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
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

        public IQsiRowValueExpressionNode[] Values { get; }

        public IQsiSetColumnExpressionNode[] SetValues { get; }

        public IQsiTableNode ValueTable { get; }

        public QsiDataConflictBehavior ConflictBehavior { get; }

        public IQsiDataConflictActionNode ConflictAction { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => 
            TreeHelper.YieldChildren(Directives, Target, Columns)
                .Concat(Values)
                .Concat(SetValues)
                .Concat(TreeHelper.YieldChildren(ValueTable, ConflictAction));

        public ImmutableDataInsertActionNode(
            IQsiTreeNode parent,
            IQsiTableDirectivesNode directives,
            IQsiTableAccessNode target,
            QsiQualifiedIdentifier[] partitions,
            IQsiColumnsDeclarationNode columns,
            IQsiRowValueExpressionNode[] values,
            IQsiSetColumnExpressionNode[] setValues,
            IQsiTableNode valueTable,
            QsiDataConflictBehavior conflictBehavior,
            IQsiDataConflictActionNode conflictAction, 
            IUserDataHolder userData)
        {
            Parent = parent;
            Directives = directives;
            Target = target;
            Partitions = partitions;
            Columns = columns;
            Values = values;
            SetValues = setValues;
            ValueTable = valueTable;
            ConflictBehavior = conflictBehavior;
            ConflictAction = conflictAction;
            UserData = userData;
        }
    }
}
