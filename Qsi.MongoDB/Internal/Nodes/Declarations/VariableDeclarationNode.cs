namespace Qsi.MongoDB.Internal.Nodes
{
    public class VariableDeclarationNode : BaseNode, IDeclarationNode
    {
        public VariableDeclaratorNode[] Declarations { get; set; }
     
        // var, let, const
        public string Kind { get; set; }
    }
}