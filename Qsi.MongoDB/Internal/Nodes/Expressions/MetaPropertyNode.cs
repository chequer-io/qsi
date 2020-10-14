namespace Qsi.MongoDB.Internal.Nodes
{
    public class MetaPropertyNode : BaseNode, IExpressionNode
    {
        public IdentifierNode Meta { get; set; }

        public IdentifierNode Property { get; set; }
    }
}