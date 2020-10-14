namespace Qsi.MongoDB.Internal.Nodes
{
    public class ImportSpecifierNode : BaseNode, IModuleSpecifierNode
    {
        public IdentifierNode Imported { get; set; }
    }
}
