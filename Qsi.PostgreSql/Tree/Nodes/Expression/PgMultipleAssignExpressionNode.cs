using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgMultipleAssignExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

    public int NColumns { get; set; }

    public int ColumnNumber { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

    public PgMultipleAssignExpressionNode()
    {
        Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
