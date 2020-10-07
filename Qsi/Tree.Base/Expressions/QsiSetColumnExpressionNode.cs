using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiSetColumnExpressionNode : QsiExpressionNode, IQsiSetColumnExpressionNode
    {
        public QsiQualifiedIdentifier Target { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

        #region Explicit
        IQsiExpressionNode IQsiSetColumnExpressionNode.Value => Value.Value;
        #endregion

        public QsiSetColumnExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
