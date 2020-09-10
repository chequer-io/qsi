using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiInvokeExpressionNode : QsiExpressionNode, IQsiInvokeExpressionNode
    {
        public QsiTreeNodeProperty<QsiFunctionAccessExpressionNode> Member { get; }

        public QsiTreeNodeList<QsiExpressionNode> Parameters { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (Member != null)
                    yield return Member.Value;

                yield return _parameters;
            }
        }

        #region Explicit
        IQsiFunctionAccessExpressionNode IQsiInvokeExpressionNode.Member => Member.Value;

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
