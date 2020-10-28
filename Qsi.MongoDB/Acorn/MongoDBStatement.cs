using System;
using Qsi.MongoDB.Internal.Nodes.Location;

namespace Qsi.MongoDB.Acorn
{
    public struct MongoDBStatement
    {
        public Range Range { get; set; }
        
        public Location Start { get; set; }
        
        public Location End { get; set; }
    }
}
