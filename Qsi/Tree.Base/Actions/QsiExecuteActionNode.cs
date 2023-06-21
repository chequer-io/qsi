using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiExecutePrepareActionNode : QsiActionNode, IQsiExecutePrepareActionNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public QsiTreeNodeProperty<QsiMultipleExpressionNode> Variables { get; }

    IQsiMultipleExpressionNode IQsiExecutePrepareActionNode.Variables => Variables.Value;

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Variables.Value);

    public QsiExecutePrepareActionNode()
    {
        Variables = new QsiTreeNodeProperty<QsiMultipleExpressionNode>(this);
    }
}