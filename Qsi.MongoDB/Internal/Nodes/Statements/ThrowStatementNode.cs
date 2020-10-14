namespace Qsi.MongoDB.Internal.Nodes
{
    public class ThrowStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Argument { get; set; }
    }
}