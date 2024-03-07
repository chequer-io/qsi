using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree;

public class TrinoTypeConstructorExpressionNode : QsiExpressionNode
{
    public QsiIdentifier Name { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

    public TrinoTypeConstructorExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}