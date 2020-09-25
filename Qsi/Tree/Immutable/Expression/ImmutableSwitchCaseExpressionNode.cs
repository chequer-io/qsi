using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableSwitchCaseExpressionNode : IQsiSwitchCaseExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode Condition { get; }

        public IQsiExpressionNode Consequent { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Condition, Consequent);

        public ImmutableSwitchCaseExpressionNode(
            IQsiTreeNode parent,
            IQsiExpressionNode condition,
            IQsiExpressionNode consequent)
        {
            Parent = parent;
            Condition = condition;
            Consequent = consequent;
        }
    }
}
