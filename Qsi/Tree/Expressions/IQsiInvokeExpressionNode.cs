namespace Qsi.Tree
{
    public interface IQsiInvokeExpressionNode : IQsiExpressionNode
    {
        IQsiFunctionAccessExpressionNode Member { get; }

        IQsiParametersExpressionNode Parameters { get; }
    }
}
