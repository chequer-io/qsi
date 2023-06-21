using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiSetVariableExpressionNode : IQsiExpressionNode
{
    QsiQualifiedIdentifier Target { get; }

    QsiAssignmentKind AssignmentKind { get; }

    IQsiExpressionNode Value { get; }
}