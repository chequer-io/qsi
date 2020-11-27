namespace Qsi.Tree
{
    public interface IQsiBinaryExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Left { get; }

        string Operator { get; }

        IQsiExpressionNode Right { get; }
    }
}
