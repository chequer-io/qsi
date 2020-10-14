namespace Qsi.MongoDB.Internal.Nodes
{
    public class YieldExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode Argument { get; set; }
        
        public bool Delegate { get; set; }
    }
}