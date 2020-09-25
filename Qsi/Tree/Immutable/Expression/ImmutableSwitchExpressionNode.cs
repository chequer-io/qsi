using System.Collections.Generic;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableSwitchExpressionNode : IQsiSwitchExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode Value { get; }

        public IQsiSwitchCaseExpressionNode[] Cases { get; }

        public IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (Value != null)
                    yield return Value;

                foreach (var @case in Cases)
                    yield return @case;
            }
        }

        public ImmutableSwitchExpressionNode(IQsiTreeNode parent, IQsiExpressionNode value, IQsiSwitchCaseExpressionNode[] cases)
        {
            Parent = parent;
            Value = value;
            Cases = cases;
        }
    }
}
