namespace Qsi.MongoDB.Internal.Nodes
{
    public class ContinueStatementNode : BaseNode, IStatementNode
    {
        public IdentifierNode Label { get; set; }
    }
}