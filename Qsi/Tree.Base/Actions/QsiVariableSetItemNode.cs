using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiVariableSetItemNode : QsiTreeNode, IQsiVariableSetItemNode
{
    public QsiIdentifier Name { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    IQsiExpressionNode IQsiVariableSetItemNode.Expression => Expression.Value;

    public QsiVariableSetItemNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);
}
