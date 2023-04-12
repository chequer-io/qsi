using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiCreateUserActionNode : IQsiActionNode
{
    QsiDataConflictBehavior ConflictBehavior { get; } 
    
    IQsiUserNode[] Users { get; }
}
