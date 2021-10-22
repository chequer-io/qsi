using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiChangeSearchPathActionNode : IQsiActionNode
    {
        QsiQualifiedIdentifier[] Identifiers { get; }
    }
}
