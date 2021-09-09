using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public class TrinoLambdaExpressionNode : QsiExpressionNode
    {
        public QsiIdentifier[] Identifiers { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public TrinoLambdaExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
