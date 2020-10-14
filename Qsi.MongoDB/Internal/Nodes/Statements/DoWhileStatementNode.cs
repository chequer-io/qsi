namespace Qsi.MongoDB.Internal.Nodes
{
    public class DoWhileStatementNode : BaseNode, IStatementNode
    {
        public IStatementNode Body { get; set; }
        
        public IExpressionNode Test { get; set; }
    }
}