namespace Qsi.MongoDB.Internal.Nodes
{
    public class SpreadElementNode : BaseNode, INode
    {
        public IExpressionNode Argument { get; set; }
    }
}