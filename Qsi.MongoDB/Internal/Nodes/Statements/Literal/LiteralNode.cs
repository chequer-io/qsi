namespace Qsi.MongoDB.Internal.Nodes;

internal class LiteralNode : BaseNode, ILiteralNode
{
    // MultiType: string, boolean, number
    public object Value { get; set; }

    public string Raw { get; set; }
}
