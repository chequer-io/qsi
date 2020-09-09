using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiMultipleExpressionNode : QsiExpressionNode, IQsiMultipleExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Elements { get; }

        #region Explicit
        IQsiExpressionNode[] IQsiMultipleExpressionNode.Elements => Elements.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public QsiMultipleExpressionNode()
        {
            Elements = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
