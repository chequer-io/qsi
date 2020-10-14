namespace Qsi.MongoDB.Internal.Nodes
{
    public class TryStatementNode : BaseNode, IStatementNode
    {
        public BlockStatementNode Block { get; set; }
        
        public CatchClauseNode Handler { get; set; }
        
        public BlockStatementNode Finalizer { get; set; }
    }
}