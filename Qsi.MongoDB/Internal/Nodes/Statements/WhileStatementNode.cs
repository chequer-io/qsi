namespace Qsi.MongoDB.Internal.Nodes
{
    public class WhileStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Test { get; set; }
        
        public IStatementNode Body { get; set; }
    }
}