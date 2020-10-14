namespace Qsi.MongoDB.Internal.Nodes
{
    public class AwaitExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode Argument { get; set; }
    }
}
