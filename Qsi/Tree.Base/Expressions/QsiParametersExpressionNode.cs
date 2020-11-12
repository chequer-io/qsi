using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiParametersExpressionNode : QsiExpressionNode, IQsiParametersExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Expressions { get; }

        public override IEnumerable<IQsiTreeNode> Children => Expressions;

        #region Explicit
        IQsiExpressionNode[] IQsiParametersExpressionNode.Expressions => Expressions.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public QsiParametersExpressionNode()
        {
            Expressions = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
