using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableColumnsDeclarationNode : IQsiColumnsDeclarationNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode[] Columns { get; }

        public IEnumerable<IQsiTreeNode> Children => Columns;

        public ImmutableColumnsDeclarationNode(IQsiTreeNode parent, IQsiColumnNode[] columns)
        {
            Parent = parent;
            Columns = columns;
        }
    }
}
