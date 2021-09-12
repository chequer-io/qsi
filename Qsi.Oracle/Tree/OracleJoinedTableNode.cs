using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleJoinedTableNode : QsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> OnCondition { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Left, Right, PivotColumns, OnCondition);

        public OracleJoinedTableNode()
        {
            OnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
