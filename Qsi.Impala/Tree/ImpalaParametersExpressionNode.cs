using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaParametersExpressionNode : QsiExpressionNode, IQsiParametersExpressionNode
    {
        public ImpalaTreeNodeList<QsiExpressionNode> Expressions { get; }

        public override IEnumerable<IQsiTreeNode> Children => Expressions;

        #region Explicit
        IQsiExpressionNode[] IQsiParametersExpressionNode.Expressions => Expressions.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public ImpalaParametersExpressionNode()
        {
            Expressions = new ImpalaTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
