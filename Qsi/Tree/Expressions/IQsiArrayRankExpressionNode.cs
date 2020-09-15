namespace Qsi.Tree
{
    public interface IQsiArrayRankExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Array { get; }

        IQsiExpressionNode Rank { get; }
    }
}
