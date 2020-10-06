namespace Qsi.Tree
{
    public interface IQsiAssignExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Target { get; }

        string Operator { get; }

        IQsiExpressionNode Value { get; }
    }
}
