using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiPrepareActionNode : IQsiActionNode
    {
        QsiQualifiedIdentifier Identifier { get; }

        IQsiExpressionNode Query { get; }
    }
}
