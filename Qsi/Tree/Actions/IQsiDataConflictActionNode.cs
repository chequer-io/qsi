using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiDataConflictActionNode : IQsiActionNode
{
    QsiQualifiedIdentifier Target { get; }

    IQsiSetColumnExpressionNode[] SetValues { get; }

    IQsiExpressionNode Condition { get; }
}