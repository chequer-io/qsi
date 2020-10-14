namespace Qsi.MongoDB.Internal.Nodes
{
    public class UnaryExpressionNode : BaseNode, IExpressionNode
    {
        public string Operator { get; set; }
        
        public bool Prefix { get; set; }
        
        public IExpressionNode Argument { get; set; }
    }
}