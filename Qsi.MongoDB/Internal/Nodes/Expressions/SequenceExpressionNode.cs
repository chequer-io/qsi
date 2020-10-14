namespace Qsi.MongoDB.Internal.Nodes
{
    public class SequenceExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode[] Expressions { get; set; }
    }
}