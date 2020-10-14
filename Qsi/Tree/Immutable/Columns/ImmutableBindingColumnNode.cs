using System.Collections.Generic;
using System.Linq;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableBindingColumnNode : IQsiBindingColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public string Id { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableBindingColumnNode(IQsiTreeNode parent, string id, IUserDataHolder userData)
        {
            Parent = parent;
            Id = id;
            UserData = userData;
        }
    }
}
