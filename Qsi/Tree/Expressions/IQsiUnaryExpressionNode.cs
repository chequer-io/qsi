namespace Qsi.Tree
{
    public interface IQsiUnaryExpressionNode : IQsiExpressionNode
    {
        string Operator { get; }

        IQsiExpressionNode Expression { get; }
    }
}
