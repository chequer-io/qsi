using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Tree.Immutable
{
    public readonly struct CompositeTableNodeProxy : IQsiCompositeTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode[] Sources { get; }

        public IEnumerable<IQsiTreeNode> Children => Sources;

        public CompositeTableNodeProxy(IQsiTreeNode parent, IQsiTableNode[] sources)
        {
            Parent = parent;
            Sources = sources;
        }
    }
}
