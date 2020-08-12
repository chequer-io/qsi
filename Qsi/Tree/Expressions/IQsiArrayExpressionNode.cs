namespace Qsi.Tree
{
    public interface IQsiArrayExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode[] Elements { get; }
    }
}
