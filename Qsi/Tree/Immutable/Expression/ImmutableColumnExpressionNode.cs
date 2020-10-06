using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableColumnExpressionNode : IQsiColumnExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode Column { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Column);

        public ImmutableColumnExpressionNode(
            IQsiTreeNode parent,
            IQsiColumnNode column)
        {
            Parent = parent;
            Column = column;
        }
    }
}
