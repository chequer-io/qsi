using System.Collections.Generic;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableTableDirectivesNode : IQsiTableDirectivesNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiDerivedTableNode[] Tables { get; }

        public bool IsRecursive { get; }

        public IEnumerable<IQsiTreeNode> Children => Tables;

        public ImmutableTableDirectivesNode(IQsiTreeNode parent, IQsiDerivedTableNode[] tables, bool isRecursive)
        {
            Parent = parent;
            Tables = tables;
            IsRecursive = isRecursive;
        }
    }
}
