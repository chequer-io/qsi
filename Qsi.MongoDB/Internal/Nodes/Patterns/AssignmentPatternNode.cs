namespace Qsi.MongoDB.Internal.Nodes
{
    public class AssignmentPatternNode : BaseNode, IPatternNode
    {
        public IPatternNode Left { get; set; }
        
        public IExpressionNode Right { get; set; }
    }
}