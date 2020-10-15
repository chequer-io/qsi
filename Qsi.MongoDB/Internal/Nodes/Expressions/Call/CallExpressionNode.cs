namespace Qsi.MongoDB.Internal.Nodes
{
    public class CallExpressionNode : BaseNode, IExpressionNode
    {
        // BaseExpression, Super
        public INode Callee { get; set; }
        
        // BaseExpression, SpreadElement
        public INode[] Arguments { get; set; }
    }
}