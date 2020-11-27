using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public class ImmutableGroupingExpressionNode: IQsiGroupingExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode[] Items { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Items;

        public ImmutableGroupingExpressionNode(
            IQsiTreeNode parent,
            IQsiExpressionNode[] items,
            IUserDataHolder userData)
        {
            Parent = parent;
            Items = items;
            UserData = userData;
        }
    }
}
