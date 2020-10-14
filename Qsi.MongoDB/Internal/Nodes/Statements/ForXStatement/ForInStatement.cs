namespace Qsi.MongoDB.Internal.Nodes
{
    public class ForInStatement : BaseNode, IForXStatementNode
    {
        public IForXStatementLeftNode Left { get; set; }
        
        public IExpressionNode Right { get; set; }
        
        public IStatementNode Body { get; set; }
    }
}