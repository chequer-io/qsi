namespace Qsi.MongoDB.Internal.Nodes
{
    public class ObjectExpressionNode : BaseNode, IExpressionNode
    {
        // Property, SpreadElement
        public INode[] Properties { get; set; }
    }
}