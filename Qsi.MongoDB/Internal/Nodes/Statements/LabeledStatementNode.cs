namespace Qsi.MongoDB.Internal.Nodes
{
    public class LabeledStatementNode : BaseNode, IStatementNode
    {
        public IdentifierNode Label { get; set; }
        
        public IStatementNode Body { get; set; }
    }
}