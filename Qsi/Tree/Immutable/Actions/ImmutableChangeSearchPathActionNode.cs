using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableChangeSearchPathActionNode : IQsiChangeSearchPathActionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiIdentifier[] Identifiers { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableChangeSearchPathActionNode(
            IQsiTreeNode parent,
            QsiIdentifier[] identifiers,
            IUserDataHolder userData)
        {
            Parent = parent;
            Identifiers = identifiers;
            UserData = userData;
        }
    }
}
