using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiAliasNode : IQsiTreeNode
    {
        QsiIdentifier Name { get; }
    }
}
