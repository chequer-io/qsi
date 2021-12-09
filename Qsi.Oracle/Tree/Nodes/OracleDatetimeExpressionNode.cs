using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleDatetimeExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> TimeZone { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression, TimeZone);

        public OracleDatetimeExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            TimeZone = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
