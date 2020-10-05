using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableAllColumnNode : IQsiAllColumnNode, IQsiTerminalNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Path { get; }

        public bool IncludeInvisibleColumns { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableAllColumnNode(IQsiTreeNode parent, QsiQualifiedIdentifier path)
        {
            Parent = parent;
            Path = path;
            IncludeInvisibleColumns = false;
        }
    }
}
