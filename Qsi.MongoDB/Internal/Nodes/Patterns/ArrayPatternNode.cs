namespace Qsi.MongoDB.Internal.Nodes
{
    public class ArrayPatternNode : BaseNode, IPatternNode
    {
        public IPatternNode[] Elements { get; set; }
    }
}