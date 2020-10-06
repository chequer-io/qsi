using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiAssignExpressionNode : QsiExpressionNode, IQsiAssignExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Target { get; }

        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Target, Value);

        #region Explicit
        IQsiExpressionNode IQsiAssignExpressionNode.Target => Target.Value;

        IQsiExpressionNode IQsiAssignExpressionNode.Value => Value.Value;
        #endregion

        public QsiAssignExpressionNode()
        {
            Target = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
