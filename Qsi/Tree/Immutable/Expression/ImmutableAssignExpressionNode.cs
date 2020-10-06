using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableAssignExpressionNode : IQsiAssignExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode Target { get; }

        public string Operator { get; }

        public IQsiExpressionNode Value { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target, Value);

        public ImmutableAssignExpressionNode(
            IQsiTreeNode parent,
            IQsiVariableAccessExpressionNode target,
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
