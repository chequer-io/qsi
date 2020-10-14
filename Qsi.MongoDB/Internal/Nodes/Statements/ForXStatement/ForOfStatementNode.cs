namespace Qsi.MongoDB.Internal.Nodes
{
    public class ForOfStatementNode : BaseNode, IForXStatementNode
    {
        public IForXStatementLeftNode Left { get; set; }
        
        public IExpressionNode Right { get; set; }
        
        public IStatementNode Body { get; set; }
        
        public bool Await { get; set; }
    }
}