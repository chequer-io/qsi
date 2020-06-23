using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiAliasedNode : IQsiTreeNode
    {
        QsiQualifiedIdentifier Alias { get; }
    }
}
