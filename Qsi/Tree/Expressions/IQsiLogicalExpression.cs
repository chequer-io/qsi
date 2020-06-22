namespace Qsi.Tree
{
    public interface IQsiLogicalExpression : IQsiExpression
    {
        IQsiExpression Left { get; }

        string Operator { get; }

        IQsiExpression Right { get; }
    }
}
