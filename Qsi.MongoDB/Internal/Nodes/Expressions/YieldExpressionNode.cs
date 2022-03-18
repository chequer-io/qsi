using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class YieldExpressionNode : BaseNode, IExpressionNode
{
    public IExpressionNode Argument { get; set; }

    public bool Delegate { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Argument; }
    }
}
