using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ConditionalExpressionNode : BaseNode, IExpressionNode
{
    public IExpressionNode Test { get; set; }

    public IExpressionNode Alternate { get; set; }

    public IExpressionNode Consequent { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Test;
            yield return Alternate;
            yield return Consequent;
        }
    }
}
