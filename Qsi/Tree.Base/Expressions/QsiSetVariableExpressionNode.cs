using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiSetVariableExpressionNode : QsiExpressionNode, IQsiSetVariableExpressionNode
    {
        public QsiQualifiedIdentifier Target { get; set; }

        public QsiAssignmentKind AssignmentKind { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

        #region Explicit
        IQsiExpressionNode IQsiSetVariableExpressionNode.Value => Value.Value;
        #endregion

        public QsiSetVariableExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
