namespace Qsi.MongoDB.Internal.Nodes
{
    public class ArrowFunctionExpressionNode : BaseNode, IExpressionNode, IFunctionNode
    {
        public IPatternNode[] Params { get; set; }
        
        public bool Generator { get; set; }
        
        public bool Async { get; set; }
        
        public INode Body { get; set; }
        
        public bool Expression { get; set; }
    }
}