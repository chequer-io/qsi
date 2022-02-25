using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public interface INode
{
    IEnumerable<INode> Children { get; }
}
