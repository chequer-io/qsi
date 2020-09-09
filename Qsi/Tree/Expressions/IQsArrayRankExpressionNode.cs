namespace Qsi.Tree
{
    public interface IQsArrayRankExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Array { get; }

        IQsiExpressionNode Rank { get; }
    }
}
