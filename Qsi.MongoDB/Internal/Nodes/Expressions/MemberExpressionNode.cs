using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class MemberExpressionNode : BaseNode, IExpressionNode, IPatternNode
{
    // TODO: MultiType (BaseExpression, Super)
    public INode Object { get; set; }

    public IExpressionNode Property { get; set; }

    public bool Computed { get; set; }

    public bool Optional { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Object;
            yield return Property;
        }
    }
}
