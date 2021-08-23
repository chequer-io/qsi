using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleDatetimeExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> TimeZone { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleDatetimeExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            TimeZone = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
