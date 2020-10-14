namespace Qsi.MongoDB.Internal.Nodes
{
    public class ImportDeclarationNode : BaseNode, IModuleDeclarationNode
    {
        public INode[] Specifiers { get; set; }
        
        public LiteralNode Source { get; set; }
    }
}