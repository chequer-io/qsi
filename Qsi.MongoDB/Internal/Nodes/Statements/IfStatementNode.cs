namespace Qsi.MongoDB.Internal.Nodes
{
    public class IfStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Test { get; set; }
        
        public IStatementNode Consequent { get; set; }
        
        public IStatementNode Alternate { get; set; }
    }
}