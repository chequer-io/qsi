namespace Qsi.Tree;

public interface IQsiVariableSetActionNode : IQsiActionNode
{
    IQsiVariableSetItemNode[] SetItems { get; }

    IQsiTreeNode Target { get; }
}
