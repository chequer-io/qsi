using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableSequentialColumnNode : IQsiSequentialColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public int Ordinal { get; }

        public IQsiAliasNode Alias { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Alias);

        public ImmutableSequentialColumnNode(IQsiTreeNode parent, int ordinal, IQsiAliasNode alias)
        {
            Parent = parent;
            Ordinal = ordinal;
            Alias = alias;
        }
    }
}
