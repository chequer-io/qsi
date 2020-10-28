using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ProgramNode : BaseNode, INode
    {
        public string SourceType { get; set; }
        
        public INode[] Body { get; set; }

        public override IEnumerable<INode> Children => Body;
    }
}