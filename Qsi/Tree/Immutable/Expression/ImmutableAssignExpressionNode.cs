using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableAssignExpressionNode : IQsiAssignExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiVariableAccessExpressionNode Variable { get; }

        public string Operator { get; }

        public IQsiExpressionNode Value { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Variable, Value);

        public ImmutableAssignExpressionNode(
            IQsiTreeNode parent,
            IQsiVariableAccessExpressionNode variable,
            string @operator,
            IQsiExpressionNode value)
        {
            Parent = parent;
            Variable = variable;
            Operator = @operator;
            Value = value;
        }
    }
}
