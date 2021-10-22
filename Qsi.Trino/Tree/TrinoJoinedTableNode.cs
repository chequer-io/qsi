using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoJoinedTableNode : QsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> OnCondition { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Left, Right, PivotColumns, OnCondition);

        public TrinoJoinedTableNode()
        {
            OnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
