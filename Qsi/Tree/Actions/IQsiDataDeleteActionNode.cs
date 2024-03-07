using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiDataDeleteActionNode : IQsiActionNode
{
    IQsiTableNode Target { get; }

    QsiQualifiedIdentifier[] Columns { get; }
}