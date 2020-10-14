namespace Qsi.MongoDB.Internal.Nodes
{
    public class ReturnStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Argument { get; set; }
    }
}