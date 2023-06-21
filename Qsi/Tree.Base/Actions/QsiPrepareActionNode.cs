using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiPrepareActionNode : QsiActionNode, IQsiPrepareActionNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Query { get; }

    IQsiExpressionNode IQsiPrepareActionNode.Query => Query.Value;

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Query);

    public QsiPrepareActionNode()
    {
        Query = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}