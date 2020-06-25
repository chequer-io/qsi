namespace Qsi.Tree
{
    public interface IQsiSwitchCaseExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode Condition { get; }

        IQsiExpressionNode Consequent { get; }
    }
}
