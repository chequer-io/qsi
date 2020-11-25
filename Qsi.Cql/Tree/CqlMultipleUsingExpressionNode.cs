using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlMultipleUsingExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeList<CqlUsingExpressionNode> Elements { get; }

        public override IEnumerable<IQsiTreeNode> Children => Elements;

        public CqlMultipleUsingExpressionNode()
        {
            Elements = new QsiTreeNodeList<CqlUsingExpressionNode>(this);
        }
    }
}
