using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDerivedTableNode : IQsiDerivedTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableDirectivesNode Directives { get; }

        public IQsiColumnsDeclarationNode Columns { get; }

        public IQsiTableNode Source { get; }

        public IQsiAliasNode Alias { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Directives, Columns, Source, Alias);

        public ImmutableDerivedTableNode(
            IQsiTreeNode parent,
            IQsiTableDirectivesNode directives,
            IQsiColumnsDeclarationNode columns,
            IQsiTableNode source,
            IQsiAliasNode alias)
        {
            Parent = parent;
            Directives = directives;
            Columns = columns;
            Source = source;
            Alias = alias;
        }
    }
}
