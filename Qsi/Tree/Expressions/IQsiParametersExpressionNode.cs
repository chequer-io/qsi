namespace Qsi.Tree
{
    public interface IQsiParametersExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode[] Expressions { get; }
    }
}
