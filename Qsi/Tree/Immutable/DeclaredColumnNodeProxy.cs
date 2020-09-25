using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Tree.Immutable
{
    public readonly struct DeclaredColumnNodeProxy : IQsiDeclaredColumnNode, IQsiTerminalNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Name { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public DeclaredColumnNodeProxy(IQsiTreeNode parent, QsiQualifiedIdentifier name)
        {
            Parent = parent;
            Name = name;
        }
    }
}
