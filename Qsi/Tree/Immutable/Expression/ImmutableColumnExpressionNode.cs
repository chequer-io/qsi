using System.Collections.Generic;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableColumnExpressionNode : IQsiColumnExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode Column { get; }

        public IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                yield return Column;
            }
        }

        public ImmutableColumnExpressionNode(IQsiTreeNode parent, IQsiColumnNode column)
        {
            Parent = parent;
            Column = column;
        }
    }
}
