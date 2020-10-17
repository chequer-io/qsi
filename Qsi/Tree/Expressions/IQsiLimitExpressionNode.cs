namespace Qsi.Tree
{
    public interface IQsiLimitExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Limit { get; }

        IQsiExpressionNode Offset { get; }
    }
}
