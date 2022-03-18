using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class MetaPropertyNode : BaseNode, IExpressionNode
{
    public IdentifierNode Meta { get; set; }

    public IdentifierNode Property { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Meta;
            yield return Property;
        }
    }
}
