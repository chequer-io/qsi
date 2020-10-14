namespace Qsi.MongoDB.Internal.Nodes
{
    public class SimpleCallExpression : BaseNode, ICallExpressionNode
    {
        public IBaseCallExpressionCalleeNode Callee { get; set; }
        
        public IBaseCallExpressionArgumentNode Arguments { get; set; }
        
        public bool Optional { get; set; }
    }
}