namespace Qsi.MongoDB.Internal.Nodes
{
    public class ClassDeclarationNode : BaseNode, IClassNode, IDeclarationNode
    {
        public IdentifierNode Id { get; set; }

        public IExpressionNode SuperClass { get; set; }

        public ClassBodyNode Body { get; set; }
    }
}