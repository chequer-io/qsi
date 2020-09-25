using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableTableExpressionNode : IQsiTableExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode Table { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Table);

        public ImmutableTableExpressionNode(IQsiTreeNode parent, IQsiTableNode table)
        {
            Parent = parent;
            Table = table;
        }
    }
}
