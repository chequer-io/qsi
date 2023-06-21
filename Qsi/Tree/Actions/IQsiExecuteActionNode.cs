using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiExecutePrepareActionNode : IQsiActionNode
{
    QsiQualifiedIdentifier Identifier { get; }

    IQsiMultipleExpressionNode Variables { get; }
}