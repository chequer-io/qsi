using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiArrayRankExpressionNode : QsiExpressionNode, IQsiArrayRankExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Array { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Rank { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Array, Rank);

        #region Explicit
        IQsiExpressionNode IQsiArrayRankExpressionNode.Array => Array.Value;

        IQsiExpressionNode IQsiArrayRankExpressionNode.Rank => Rank.Value;
        #endregion

        public QsiArrayRankExpressionNode()
        {
            Array = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Rank = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
