namespace Qsi.MongoDB.Internal.Nodes
{
    public class VariableDeclaratorNode : BaseNode, INode
    {
        public IPatternNode Id { get; set; }
        
        public IExpressionNode Init { get; set; }
    }
}