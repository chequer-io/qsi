namespace Qsi.MongoDB.Internal.Nodes
{
    public class TaggedTemplateExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode Tag { get; set; }
        
        public TemplateLiteralNode Quasi { get; set; }
    }
}