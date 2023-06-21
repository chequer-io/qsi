using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiColumnExpressionNode : QsiExpressionNode, IQsiColumnExpressionNode
{
    public QsiTreeNodeProperty<QsiColumnNode> Column { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Column);

    #region Explicit
    IQsiColumnNode IQsiColumnExpressionNode.Column => Column.Value;
    #endregion

    public QsiColumnExpressionNode()
    {
        Column = new QsiTreeNodeProperty<QsiColumnNode>(this);
    }
}