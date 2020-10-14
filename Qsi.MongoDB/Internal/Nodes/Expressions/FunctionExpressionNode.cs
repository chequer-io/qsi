namespace Qsi.MongoDB.Internal.Nodes
{
    public class FunctionExpressionNode : BaseNode, IFunctionNode, IExpressionNode
    {
        public IPatternNode[] Params { get; set; }
        
        public bool Generator { get; set; }
        
        public bool Async { get; set; }
        
        public INode Body { get; set; }
    }
}