namespace Qsi.MongoDB.Internal.Nodes
{
    public class RestElementNode : BaseNode, IPatternNode
    {
        public IPatternNode Argument { get; set; }
    }
}