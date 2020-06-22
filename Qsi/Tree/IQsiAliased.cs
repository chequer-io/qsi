using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiAliased : IQsiTreeNode
    {
        QsiQualifiedIdentifier Alias { get; }
    }
}
