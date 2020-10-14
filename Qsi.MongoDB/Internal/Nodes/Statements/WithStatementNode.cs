namespace Qsi.MongoDB.Internal.Nodes
{
    public class WithStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Object { get; set; }
        
        public IStatementNode Body { get; set; }
    }
}