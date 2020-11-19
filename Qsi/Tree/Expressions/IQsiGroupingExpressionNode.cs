namespace Qsi.Tree
{
    public interface IQsiGroupingExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode[] Items { get; }

        IQsiExpressionNode Having { get; }
    }
}
