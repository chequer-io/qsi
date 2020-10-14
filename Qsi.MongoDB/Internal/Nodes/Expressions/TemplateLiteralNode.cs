namespace Qsi.MongoDB.Internal.Nodes
{
    public class TemplateLiteralNode : BaseNode, IExpressionNode
    {
        public TemplateElementNode[] Quasis { get; set; }
        
        public IExpressionNode[] Expressions { get; set; }
    }
}