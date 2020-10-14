namespace Qsi.MongoDB.Internal.Nodes
{
    public class ClassExpressionNode : BaseNode, IClassNode, IExpressionNode
    {
        public IExpressionNode SuperClass { get; set; }
        
        public ClassBodyNode Body { get; set; }
        
        public IdentifierNode Id { get; set; }
    }
}