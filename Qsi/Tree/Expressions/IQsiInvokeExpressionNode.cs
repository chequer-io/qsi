namespace Qsi.Tree
{
    public interface IQsiInvokeExpressionNode : IQsiExpressionNode
    {
        IQsiFunctionExpressionNode Member { get; }

        IQsiParametersExpressionNode Parameters { get; }
    }
}
