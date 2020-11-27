namespace Qsi.Tree
{
    public interface IQsiGroupingExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode[] Items { get; }
    }
}
