namespace Qsi.MongoDB.Internal.Nodes
{
    public class TemplateElementNode : BaseNode, INode
    {
        public bool Tail { get; set; }
        
        public TemplateElementValue Value { get; set; }
    }

    // TODO: Implicit casting
    public class TemplateElementValue
    {
        public string Cooked { get; set; }
        
        public string Raw { get; set; }
    }
}