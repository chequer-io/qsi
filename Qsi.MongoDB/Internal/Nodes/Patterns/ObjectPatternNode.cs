namespace Qsi.MongoDB.Internal.Nodes
{
    public class ObjectPatternNode : BaseNode, IPatternNode
    {
        // TODO: Multi Type AssignmentProperty, RestElement
        public object[] Properties { get; set; }
    }
}