using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiExecuteActionNode : IQsiActionNode
    {
        QsiQualifiedIdentifier Identifier { get; }

        IQsiMultipleExpressionNode Variables { get; }
    }
}
