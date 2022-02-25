using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ArrowFunctionExpressionNode : BaseNode, IExpressionNode, IFunctionNode
{
    public bool Expression { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            foreach (var param in Params)
                yield return param;

            yield return Body;
        }
    }

    public IPatternNode[] Params { get; set; }

    public bool Generator { get; set; }

    public bool Async { get; set; }

    public INode Body { get; set; }
}
