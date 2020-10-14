namespace Qsi.MongoDB.Internal.Nodes
{
    public class CatchClauseNode : BaseNode, INode
    {
        public IPatternNode Param { get; set; }
        
        public BlockStatementNode Body { get; set; }
    }
}