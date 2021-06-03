namespace Qsi.Tree
{
    public interface IQsiDataUpdateActionNode : IQsiActionNode
    {
        IQsiTableNode Target { get; }

        IQsiRowValueExpressionNode Value { get; }

        IQsiSetColumnExpressionNode[] SetValues { get; }
    }
}
