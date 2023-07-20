using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiMemberExpressionNode : IQsiExpressionNode
{
    QsiQualifiedIdentifier Identifier { get; }
}