using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiArrayRankExpressionNode : QsiExpressionNode, IQsArrayRankExpressionNode
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
        IQsiExpressionNode IQsArrayRankExpressionNode.Array => Array.Value;

        IQsiExpressionNode IQsArrayRankExpressionNode.Rank => Rank.Value;
        #endregion

        public QsiArrayRankExpressionNode()
        {
            Array = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Rank = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
