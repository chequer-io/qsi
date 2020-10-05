using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDropPrepareActionNode : IQsiDropPrepareActionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Identifier { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableDropPrepareActionNode(IQsiTreeNode parent, QsiQualifiedIdentifier identifier) 
        {
            Parent = parent;
            Identifier = identifier;
        }
    }
}
