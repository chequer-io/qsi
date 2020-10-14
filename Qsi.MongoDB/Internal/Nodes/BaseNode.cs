using System;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class BaseNode : INode
    {
        public int Start { get; set; }
        
        public int End { get; set; }

        public Range Range => Start..End;
    }
}