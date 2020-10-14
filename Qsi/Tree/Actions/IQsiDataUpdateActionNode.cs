namespace Qsi.Tree
{
    public interface IQsiDataUpdateActionNode : IQsiActionNode
    {
        IQsiTableAccessNode Target { get; }

        IQsiSetColumnExpressionNode[] SetValues { get; }

        IQsiWhereExpressionNode WhereExpression { get; }

        IQsiMultipleOrderExpressionNode OrderExpression { get; }

        IQsiLimitExpressionNode LimitExpression { get; }
    }
}
