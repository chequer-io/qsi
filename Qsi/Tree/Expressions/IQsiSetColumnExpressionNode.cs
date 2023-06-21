using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiSetColumnExpressionNode : IQsiExpressionNode
{
    QsiQualifiedIdentifier Target { get; }

    IQsiExpressionNode Value { get; }
}