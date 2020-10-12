using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableColumnsDeclarationNode : IQsiColumnsDeclarationNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode[] Columns { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Columns;

        public ImmutableColumnsDeclarationNode(IQsiTreeNode parent, IQsiColumnNode[] columns, IUserDataHolder userData)
        {
            Parent = parent;
            Columns = columns;
            UserData = userData;
        }
    }
}
