using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class RestElementNode : BaseNode, IPatternNode
{
    public IPatternNode Argument { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Argument; }
    }
}
