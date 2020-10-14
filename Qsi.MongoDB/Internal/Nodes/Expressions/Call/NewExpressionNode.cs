namespace Qsi.MongoDB.Internal.Nodes
{
    public class NewExpressionNode : BaseNode, ICallExpressionNode
    {
        public IBaseCallExpressionCalleeNode Callee { get; set; }
        
        public IBaseCallExpressionArgumentNode Arguments { get; set; }
    }
}