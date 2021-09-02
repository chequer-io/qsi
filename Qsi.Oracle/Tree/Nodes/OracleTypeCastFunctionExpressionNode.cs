using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleTypeCastFunctionExpressionNode : OracleInvokeExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> DefaultExpressionOnError { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(base.Children, DefaultExpressionOnError);

        public OracleTypeCastFunctionExpressionNode()
        {
            DefaultExpressionOnError = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
