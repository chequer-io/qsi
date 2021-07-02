using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiTableFunctionNode : QsiTableNode, IQsiTableFunctionNode
    {
        public QsiTreeNodeProperty<QsiFunctionExpressionNode> Member { get; }

        public QsiTreeNodeList<QsiExpressionNode> Parameters { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Member?.Value, _parameters);

        #region Explicit
        IQsiFunctionExpressionNode IQsiInvokeExpressionNode.Member => Member.Value;

        IQsiParametersExpressionNode IQsiInvokeExpressionNode.Parameters => _parameters;
        #endregion

        private readonly QsiParametersExpressionNode _parameters;

        public QsiTableFunctionNode()
        {
            Member = new QsiTreeNodeProperty<QsiFunctionExpressionNode>(this);

            _parameters = new QsiParametersExpressionNode
            {
                Parent = this
            };

            Parameters = _parameters.Expressions;
        }
    }
}
