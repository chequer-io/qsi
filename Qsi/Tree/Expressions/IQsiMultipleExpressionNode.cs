namespace Qsi.Tree
{
    public interface IQsiMultipleExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode[] Elements { get; }
    }
}
