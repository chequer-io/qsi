using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiLimitExpressionNode : QsiExpressionNode, IQsiLimitExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Limit { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Offset { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Limit, Offset);

    #region Explicit
    IQsiExpressionNode IQsiLimitExpressionNode.Limit => Limit.Value;

    IQsiExpressionNode IQsiLimitExpressionNode.Offset => Offset.Value;
    #endregion

    public QsiLimitExpressionNode()
    {
        Limit = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Offset = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}