using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableBindingColumnNode : IQsiBindingColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public string Id { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableBindingColumnNode(IQsiTreeNode parent, string id)
        {
            Parent = parent;
            Id = id;
        }
    }
}
