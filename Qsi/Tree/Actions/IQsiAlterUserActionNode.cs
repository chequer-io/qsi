using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiAlterUserActionNode : IQsiActionNode
{
    QsiDataConflictBehavior ConflictBehavior { get; }

    IQsiUserNode[] Users { get; }
}
