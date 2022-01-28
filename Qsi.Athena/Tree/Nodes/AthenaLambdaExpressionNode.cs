using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes;

public class AthenaLambdaExpressionNode : QsiExpressionNode
{
    public AthenaLambdaExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public QsiIdentifier[] Identifiers { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);
}
