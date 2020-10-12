using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDeclaredColumnNode : IQsiDeclaredColumnNode, IQsiTerminalNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Name { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableDeclaredColumnNode(IQsiTreeNode parent, QsiQualifiedIdentifier name, IUserDataHolder userData)
        {
            Parent = parent;
            Name = name;
            UserData = userData;
        }
    }
}
