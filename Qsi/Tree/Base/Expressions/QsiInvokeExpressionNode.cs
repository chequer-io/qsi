namespace Qsi.Tree.Base
{
    public sealed class QsiInvokeExpressionNode : QsiExpressionNode, IQsiInvokeExpressionNode
    {
        public QsiTreeNodeProperty<QsiFunctionAccessExpressionNode> Member { get; }

        public QsiTreeNodeList<QsiExpressionNode> Parameters { get; }

        #region Explicit
        IQsiFunctionAccessExpressionNode IQsiInvokeExpressionNode.Member => Member.GetValue();

        IQsiParametersExpressionNode IQsiInvokeExpressionNode.Parameters => _parameters;
        #endregion

        private readonly QsiParametersExpressionNode _parameters;

        public QsiInvokeExpressionNode()
        {
            Member = new QsiTreeNodeProperty<QsiFunctionAccessExpressionNode>(this);

            _parameters = new QsiParametersExpressionNode
            {
                Parent = this
            };

            Parameters = _parameters.Expressions;
        }
    }
}
