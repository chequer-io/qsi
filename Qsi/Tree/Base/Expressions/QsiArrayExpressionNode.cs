using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiArrayExpressionNode : QsiExpressionNode, IQsiArrayExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Elements { get; }

        #region Explicit
        IQsiExpressionNode[] IQsiArrayExpressionNode.Elements => Elements.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public QsiArrayExpressionNode()
        {
            Elements = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
