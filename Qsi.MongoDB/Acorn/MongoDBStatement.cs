using System;
using Qsi.MongoDB.Internal.Nodes.Location;

namespace Qsi.MongoDB.Acorn;

internal struct MongoDBStatement
{
    public Range Range { get; set; }
        
    public Location Start { get; set; }
        
    public Location End { get; set; }
}