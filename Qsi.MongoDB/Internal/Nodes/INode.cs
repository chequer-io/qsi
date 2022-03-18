using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal interface INode
{
    IEnumerable<INode> Children { get; }
}
