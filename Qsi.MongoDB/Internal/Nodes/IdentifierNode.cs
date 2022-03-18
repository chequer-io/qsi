namespace Qsi.MongoDB.Internal.Nodes;

internal class IdentifierNode : BaseNode, IExpressionNode, IPatternNode
{
    public string Name { get; set; }
}
