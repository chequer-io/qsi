using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataDeleteActionNode : IQsiDataDeleteActionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiTableNode Target { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target);

        public ImmutableDataDeleteActionNode(
            IQsiTreeNode parent,
            IQsiTableNode target,
            IUserDataHolder userData)
        {
            Parent = parent;
            Target = target;
            UserData = userData;
        }
    }
}
