namespace Qsi.Tree
{
    public interface IQsiUnaryExpression : IQsiExpression
    {
        string Operator { get; }

        IQsiExpression Expression { get; }
    }
}
