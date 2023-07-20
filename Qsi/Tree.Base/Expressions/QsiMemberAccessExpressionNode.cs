using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiMemberAccessExpressionNode : QsiExpressionNode, IQsiMemberAccessExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Target { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Member { get; }

    public override IEnumerable<IQsiTreeNode> Children
        => TreeHelper.YieldChildren(Target, Member);

    #region Explicit
    IQsiExpressionNode IQsiMemberAccessExpressionNode.Target => Target.Value;

    IQsiExpressionNode IQsiMemberAccessExpressionNode.Member => Member.Value;
    #endregion

    public QsiMemberAccessExpressionNode()
    {
        Target = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Member = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}