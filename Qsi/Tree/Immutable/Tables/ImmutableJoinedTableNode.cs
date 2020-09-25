using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableJoinedTableNode : IQsiJoinedTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode Left { get; }

        public QsiJoinType JoinType { get; }

        public IQsiTableNode Right { get; }

        public IQsiColumnsDeclarationNode PivotColumns { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Left, Right, PivotColumns);

        public ImmutableJoinedTableNode(
            IQsiTreeNode parent,
            IQsiTableNode left,
            QsiJoinType joinType,
            IQsiTableNode right,
            IQsiColumnsDeclarationNode pivotColumns)
        {
            Parent = parent;
            Left = left;
            JoinType = joinType;
            Right = right;
            PivotColumns = pivotColumns;
        }
    }
}
