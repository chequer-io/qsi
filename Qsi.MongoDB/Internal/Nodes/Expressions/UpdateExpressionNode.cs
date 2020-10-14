namespace Qsi.MongoDB.Internal.Nodes
{
    public class UpdateExpressionNode : BaseNode, IExpressionNode
    {
        public string Operator { get; set; }
        
        public IExpressionNode Argument { get; set; }
        
        public bool Prefix { get; set; }
    }
}