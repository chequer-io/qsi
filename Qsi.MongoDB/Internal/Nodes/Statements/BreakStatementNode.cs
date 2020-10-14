namespace Qsi.MongoDB.Internal.Nodes
{
    public class BreakStatementNode : BaseNode, IStatementNode
    {
        public IdentifierNode Label { get; set; }
    }
}