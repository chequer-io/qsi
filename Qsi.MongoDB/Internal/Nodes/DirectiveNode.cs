namespace Qsi.MongoDB.Internal.Nodes
{
    public class DirectiveNode : BaseNode, INode
    {
        public LiteralNode Expression { get; set; }
        
        public string Directive { get; set; }
    }
}