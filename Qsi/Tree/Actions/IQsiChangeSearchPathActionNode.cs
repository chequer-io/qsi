using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiChangeSearchPathActionNode : IQsiActionNode
    {
        QsiIdentifier[] Identifiers { get; }
    }
}
