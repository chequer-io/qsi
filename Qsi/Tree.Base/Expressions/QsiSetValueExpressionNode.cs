using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiSetValueExpressionNode : QsiTreeNode, IQsiSetValueExpressionNode
{
    public QsiQualifiedIdentifier Target { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

    #region Explicit
    IQsiExpressionNode IQsiSetValueExpressionNode.Value => Value.Value;
    #endregion

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

    public QsiSetValueExpressionNode()
    {
        Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
