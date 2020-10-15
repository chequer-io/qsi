using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ObjectPatternNode : BaseNode, IPatternNode
    {
        // TODO: Multi Type AssignmentProperty, RestElement
        public INode[] Properties { get; set; }

        public override IEnumerable<INode> Children => Properties;
    }
}
