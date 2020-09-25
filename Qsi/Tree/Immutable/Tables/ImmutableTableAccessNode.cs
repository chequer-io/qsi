using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableTableAccessNode : IQsiTableAccessNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Identifier { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableTableAccessNode(IQsiTreeNode parent, QsiQualifiedIdentifier identifier)
        {
            Parent = parent;
            Identifier = identifier;
        }
    }
}
