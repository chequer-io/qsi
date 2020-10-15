using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.MongoDB.Internal.Nodes.Location;

namespace Qsi.MongoDB.Internal.Nodes
{
    public abstract class BaseNode : INode
    {
        public virtual IEnumerable<INode> Children { get; } = Enumerable.Empty<INode>();
        
        public int Start { get; set; }
        
        public int End { get; set; }

        public NodeLocation Loc { get; set; }
        
        public Range Range => Start..End;
    }
}