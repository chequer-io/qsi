using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleTypeCastFunctionExpressionNode : OracleInvokeExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> DefaultExpressionOnError { get; }

        public OracleTypeCastFunctionExpressionNode()
        {
            DefaultExpressionOnError = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
