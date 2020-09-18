using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Base
{
    public sealed class QsiAssignExpressionNode : QsiExpressionNode, IQsiAssignExpressionNode
    {
        public QsiTreeNodeProperty<QsiVariableAccessExpressionNode> Variable { get; }

        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Variable, Value);

        #region Explicit
        IQsiVariableAccessExpressionNode IQsiAssignExpressionNode.Variable => Variable.Value;

        IQsiExpressionNode IQsiAssignExpressionNode.Value => Value.Value;
        #endregion

        public QsiAssignExpressionNode()
        {
            Variable = new QsiTreeNodeProperty<QsiVariableAccessExpressionNode>(this);
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
