using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiDataConflictActionNode : IQsiActionNode
    {
        QsiQualifiedIdentifier Target { get; }

        IQsiAssignExpressionNode[] Elements { get; }

        IQsiExpressionNode Condition { get; }
    }
}
