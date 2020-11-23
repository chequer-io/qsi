using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiMemberAccessExpressionNode : QsiExpressionNode, IQsiMemberAccessExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Array { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Rank { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Array, Rank);

        #region Explicit
        IQsiExpressionNode IQsiMemberAccessExpressionNode.Target => Array.Value;

        IQsiExpressionNode IQsiMemberAccessExpressionNode.Member => Rank.Value;
        #endregion

        public QsiMemberAccessExpressionNode()
        {
            Array = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Rank = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
