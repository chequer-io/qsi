using System.Collections.Generic;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableRowValueExpressionNode : IQsiRowValueExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode[] ColumnValues { get; }

        public IEnumerable<IQsiTreeNode> Children => ColumnValues;

        public ImmutableRowValueExpressionNode(IQsiTreeNode parent, IQsiExpressionNode[] columnValues)
        {
            Parent = parent;
            ColumnValues = columnValues;
        }
    }
}
