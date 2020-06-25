using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiParametersExpressionNode : QsiExpressionNode, IQsiParametersExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Expressions { get; }

        #region Explicit
        IQsiExpressionNode[] IQsiParametersExpressionNode.Expressions => Expressions.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public QsiParametersExpressionNode()
        {
            Expressions = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
