using Qsi.Tree;

namespace Qsi.Compiler.Proxy
{
    public readonly struct DerivedColumnNodeProxy : IQsiDerivedColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode Column { get; }

        public IQsiExpressionNode Expression { get; }

        public IQsiAliasNode Alias { get; }

        public DerivedColumnNodeProxy(IQsiTreeNode parent, IQsiColumnNode column, IQsiAliasNode alias)
        {
            Parent = parent;
            Column = column;
            Expression = null;
            Alias = alias;
        }

        public DerivedColumnNodeProxy(IQsiTreeNode parent, IQsiExpressionNode expression, IQsiAliasNode alias)
        {
            Parent = parent;
            Column = null;
            Expression = expression;
            Alias = alias;
        }
    }
}
