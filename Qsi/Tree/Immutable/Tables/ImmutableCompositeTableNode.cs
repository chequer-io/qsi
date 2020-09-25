using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableCompositeTableNode : IQsiCompositeTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode[] Sources { get; }

        public IEnumerable<IQsiTreeNode> Children => Sources;

        public ImmutableCompositeTableNode(IQsiTreeNode parent, IQsiTableNode[] sources)
        {
            Parent = parent;
            Sources = sources;
        }
    }
}
