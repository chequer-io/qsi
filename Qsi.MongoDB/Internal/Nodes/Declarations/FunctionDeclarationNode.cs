namespace Qsi.MongoDB.Internal.Nodes
{
    public class FunctionDeclarationNode : BaseNode, IFunctionNode, IDeclarationNode
    {
        public IPatternNode[] Params { get; set; }
        
        public bool Generator { get; set; }
        
        public bool Async { get; set; }
        
        public INode Body { get; set; }

        public IdentifierNode Id { get; set; }
    }
}