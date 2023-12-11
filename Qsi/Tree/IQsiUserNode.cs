namespace Qsi.Tree;

public interface IQsiUserNode : IQsiTreeNode
{
    string Username { get; }

    IQsiExpressionNode Password { get; }
}
