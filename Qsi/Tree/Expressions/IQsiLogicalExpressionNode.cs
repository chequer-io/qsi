namespace Qsi.Tree
{
    public interface IQsiLogicalExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Left { get; }

        string Operator { get; }

        IQsiExpressionNode Right { get; }
    }
}
