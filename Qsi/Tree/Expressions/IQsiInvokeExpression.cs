namespace Qsi.Tree
{
    public interface IQsiInvokeExpression : IQsiExpression
    {
        IQsiFunctionAccessExpression Member { get; }

        IQsiExpression[] Parameters { get; }
    }
}
