using Qsi.Tree;

namespace Qsi.Compiler.Proxy
{
    public readonly struct DerivedTableNodeProxy : IQsiDerivedTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableDirectivesNode Directives { get; }

        public IQsiColumnsDeclarationNode Columns { get; }

        public IQsiTableNode Source { get; }

        public IQsiAliasNode Alias { get; }

        public DerivedTableNodeProxy(
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
