using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableTableDirectivesNode : IQsiTableDirectivesNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiDerivedTableNode[] Tables { get; }

        public bool IsRecursive { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Tables;

        public ImmutableTableDirectivesNode(
            IQsiTreeNode parent,
            IQsiDerivedTableNode[] tables,
            bool isRecursive,
            IUserDataHolder userData)
        {
            Parent = parent;
            Tables = tables;
            IsRecursive = isRecursive;
            UserData = userData;
        }
    }
}
