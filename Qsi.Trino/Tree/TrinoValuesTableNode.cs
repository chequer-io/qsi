using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Trino.Tree;

public class TrinoValuesTableNode : QsiTableNode
{
    public QsiTreeNodeList<QsiRowValueExpressionNode> Rows { get; }

    public override IEnumerable<IQsiTreeNode> Children => Rows;

    public TrinoValuesTableNode()
    {
        Rows = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
    }
}