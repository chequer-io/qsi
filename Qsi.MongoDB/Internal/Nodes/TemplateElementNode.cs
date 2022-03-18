namespace Qsi.MongoDB.Internal.Nodes;

internal class TemplateElementNode : BaseNode, INode
{
    public bool Tail { get; set; }

    public TemplateElementValue Value { get; set; }
}

// TODO: Implicit casting
internal class TemplateElementValue
{
    public string Cooked { get; set; }

    public string Raw { get; set; }
}
