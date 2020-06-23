namespace Qsi.Tree
{
    public interface IQsiInvokeExpressionNode : IQsiExpressionNode
    {
        IQsiFunctionAccessExpressionNode Member { get; }

        IQsiExpressionNode[] Parameters { get; }
    }
}
