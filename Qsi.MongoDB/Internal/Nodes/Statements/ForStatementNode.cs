namespace Qsi.MongoDB.Internal.Nodes
{
    public class ForStatementNode : BaseNode, IStatementNode
    {
        public IForStatementInitNode Init { get; set; }
        
        public IExpressionNode Test { get; set; }
        
        public IExpressionNode Update { get; set; }
        
        public IStatementNode Body { get; set; }
    }
}