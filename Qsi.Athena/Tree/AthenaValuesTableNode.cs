using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Athena.Tree
{
    public sealed class AthenaValuesTableNode : QsiTableNode
    {
        public QsiTreeNodeList<QsiRowValueExpressionNode> Rows { get; }

        public override IEnumerable<IQsiTreeNode> Children => Rows;

        public AthenaValuesTableNode()
        {
            Rows = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
        }
    }
}
