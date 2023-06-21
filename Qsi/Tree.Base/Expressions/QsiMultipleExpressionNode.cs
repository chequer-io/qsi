using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree;

public class QsiMultipleExpressionNode : QsiExpressionNode, IQsiMultipleExpressionNode
{
    public QsiTreeNodeList<QsiExpressionNode> Elements { get; }

    public override IEnumerable<IQsiTreeNode> Children => Elements;

    #region Explicit
    IQsiExpressionNode[] IQsiMultipleExpressionNode.Elements => Elements.Cast<IQsiExpressionNode>().ToArray();
    #endregion

    public QsiMultipleExpressionNode()
    {
        Elements = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}