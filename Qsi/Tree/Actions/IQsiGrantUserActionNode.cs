namespace Qsi.Tree;

public interface IQsiGrantUserActionNode : IQsiActionNode
{
    string[] Roles { get; }

    bool AllPrivileges { get; }

    IQsiUserNode[] Users { get; }
}
