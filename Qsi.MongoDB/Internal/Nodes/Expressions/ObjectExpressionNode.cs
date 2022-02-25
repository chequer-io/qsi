using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ObjectExpressionNode : BaseNode, IExpressionNode
{
    // Property, SpreadElement
    public INode[] Properties { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            foreach (var node in Properties)
                yield return node;
        }
    }
}
