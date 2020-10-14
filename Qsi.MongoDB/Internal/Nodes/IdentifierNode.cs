namespace Qsi.MongoDB.Internal.Nodes
{
    public class IdentifierNode : BaseNode, IExpressionNode, IPatternNode
    {
        public string Name { get; set; }
    }
}