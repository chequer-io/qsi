using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ThrowStatementNode : BaseNode, IStatementNode
{
    public IExpressionNode Argument { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Argument; }
    }
}
