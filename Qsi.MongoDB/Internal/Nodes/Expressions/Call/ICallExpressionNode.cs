namespace Qsi.MongoDB.Internal.Nodes
{
    public interface ICallExpressionNode : IExpressionNode
    {
        public IBaseCallExpressionCalleeNode Callee { get; set; }
        
        public IBaseCallExpressionArgumentNode Arguments { get; set; }
    }

    // TODO: Impl to BaseExpression, Super
    public interface IBaseCallExpressionCalleeNode
    {
    }

    // TODO: Impl to BaseExpression, SpreadElement
    public interface IBaseCallExpressionArgumentNode
    {
    }
}