using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoLambdaExpressionNode : QsiExpressionNode
    {
        public QsiIdentifier[] Identifiers { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

        public TrinoLambdaExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
