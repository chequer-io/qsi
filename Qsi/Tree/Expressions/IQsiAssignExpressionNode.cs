namespace Qsi.Tree
{
    public interface IQsiAssignExpressionNode : IQsiExpressionNode
    {
        IQsiMemberAccessExpressionNode Member { get; }

        string Operator { get; }

        IQsiExpressionNode Value { get; }
    }
}
