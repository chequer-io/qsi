namespace Qsi.Tree;

public interface IQsiSwitchExpressionNode : IQsiExpressionNode
{
    IQsiExpressionNode Value { get; }

    IQsiSwitchCaseExpressionNode[] Cases { get; }
}