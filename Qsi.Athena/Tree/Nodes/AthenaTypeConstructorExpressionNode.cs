using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes
{
    public class AthenaTypeConstructorExpressionNode : QsiExpressionNode
    {
        public QsiIdentifier Name { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

        public AthenaTypeConstructorExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
