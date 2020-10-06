using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiDataConflictActionNode : IQsiActionNode
    {
        QsiQualifiedIdentifier Target { get; }

        IQsiAssignExpressionNode[] SetValues { get; }

        IQsiExpressionNode Condition { get; }
    }
}
