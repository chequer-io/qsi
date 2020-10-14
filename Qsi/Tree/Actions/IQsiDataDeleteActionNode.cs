namespace Qsi.Tree
{
    public interface IQsiDataDeleteActionNode : IQsiActionNode
    {
        IQsiTableAccessNode Target { get; }

        IQsiWhereExpressionNode WhereExpression { get; }
    }
}
