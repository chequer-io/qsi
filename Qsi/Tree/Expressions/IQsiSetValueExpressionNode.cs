using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiSetValueExpressionNode : IQsiExpressionNode
{
    QsiQualifiedIdentifier Target { get; }

    IQsiExpressionNode Value { get; }
}
