using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class ArrayPatternNode : BaseNode, IPatternNode
{
    public IPatternNode[] Elements { get; set; }

    public override IEnumerable<INode> Children => Elements;
}
