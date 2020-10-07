using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableSetColumnExpressionNode : IQsiSetColumnExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Target { get; }

        public string Operator { get; }

        public IQsiExpressionNode Value { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

        public ImmutableSetColumnExpressionNode(
            IQsiTreeNode parent,
            QsiQualifiedIdentifier target,
            string @operator,
            IQsiExpressionNode value)
        {
            Parent = parent;
            Target = target;
            Operator = @operator;
            Value = value;
        }
    }
}
