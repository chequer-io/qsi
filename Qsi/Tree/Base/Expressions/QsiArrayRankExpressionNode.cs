using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiArrayRankExpressionNode : QsiExpressionNode, IQsiArrayRankExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Array { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Rank { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Array.IsEmpty)
                    yield return Array.Value;

                if (!Rank.IsEmpty)
                    yield return Rank.Value;
            }
        }

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
