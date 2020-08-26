using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Compiler.Proxy
{
    public readonly struct AllColumnNodeProxy : IQsiAllColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Path { get; }

        public bool IncludeInvisibleColumns { get; }

        public AllColumnNodeProxy(IQsiTreeNode parent, QsiQualifiedIdentifier path)
        {
            Parent = parent;
            Path = path;
            IncludeInvisibleColumns = false;
        }
    }
}
