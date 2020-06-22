namespace Qsi.Tree
{
    public interface IQsiAssignExpression : IQsiExpression
    {
        IQsiMemberAccessExpression Member { get; }

        string Operator { get; }

        IQsiExpression Value { get; }
    }
}
