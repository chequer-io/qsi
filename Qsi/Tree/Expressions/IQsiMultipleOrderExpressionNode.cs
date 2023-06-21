namespace Qsi.Tree;

public interface IQsiMultipleOrderExpressionNode : IQsiExpressionNode
{
    IQsiOrderExpressionNode[] Orders { get; }
}