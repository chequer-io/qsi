namespace Qsi.Tree
{
    public interface IQsiAssignExpressionNode : IQsiExpressionNode
    {
        IQsiVariableAccessExpressionNode Variable { get; }

        string Operator { get; }

        IQsiExpressionNode Value { get; }
    }
}
