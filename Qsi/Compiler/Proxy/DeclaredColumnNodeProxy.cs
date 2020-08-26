using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Compiler.Proxy
{
    public readonly struct DeclaredColumnNodeProxy : IQsiDeclaredColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Name { get; }

        public DeclaredColumnNodeProxy(IQsiTreeNode parent, QsiQualifiedIdentifier name)
        {
            Parent = parent;
            Name = name;
        }
    }
}
