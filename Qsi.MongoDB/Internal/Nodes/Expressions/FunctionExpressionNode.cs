using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class FunctionExpressionNode : BaseNode, IFunctionNode, IExpressionNode
{
    public IPatternNode[] Params { get; set; }

    public bool Generator { get; set; }

    public bool Async { get; set; }

    public INode Body { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            foreach (var node in Params)
                yield return node;

            yield return Body;
        }
    }
}
