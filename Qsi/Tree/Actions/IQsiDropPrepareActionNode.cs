using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiDropPrepareActionNode : IQsiActionNode
    {
        QsiQualifiedIdentifier Identifier { get; }
    }
}
