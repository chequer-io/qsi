namespace Qsi.MongoDB.Internal.Nodes
{
    public class ExportNamedDeclarationNode : BaseNode, IModuleDeclarationNode
    {
        public IDeclarationNode Declaration { get; set; }
        
        public ExportSpecifierNode[] Specifiers { get; set; }
        
        public LiteralNode Source { get; set; }
    }
}
