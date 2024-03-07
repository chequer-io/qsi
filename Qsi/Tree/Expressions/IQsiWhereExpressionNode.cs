namespace Qsi.Tree;

public interface IQsiWhereExpressionNode : IQsiExpressionNode
{
    IQsiExpressionNode Expression { get; }
}