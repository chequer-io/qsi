namespace Qsi.Tree
{
    public interface IQsiDataUpdateActionNode : IQsiActionNode
    {
        IQsiTableNode Target { get; }

        IQsiSetColumnExpressionNode[] SetValues { get; }
    }
}
