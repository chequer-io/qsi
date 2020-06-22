namespace Qsi.Tree
{
    public interface IQsiTableExpression : IQsiExpression
    {
        IQsiTable Table { get; }
    }
}
