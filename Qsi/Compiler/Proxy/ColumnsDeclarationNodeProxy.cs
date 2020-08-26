using Qsi.Tree;

namespace Qsi.Compiler.Proxy
{
    public readonly struct ColumnsDeclarationNodeProxy : IQsiColumnsDeclarationNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode[] Columns { get; }

        public ColumnsDeclarationNodeProxy(IQsiTreeNode parent, IQsiColumnNode[] columns)
        {
            Parent = parent;
            Columns = columns;
        }
    }
}
