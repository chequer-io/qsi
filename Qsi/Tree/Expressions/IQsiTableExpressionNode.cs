namespace Qsi.Tree;

public interface IQsiTableExpressionNode : IQsiExpressionNode
{
    IQsiTableNode Table { get; }
}