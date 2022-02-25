using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class WhileStatementNode : BaseNode, IStatementNode
{
    public IExpressionNode Test { get; set; }

    public IStatementNode Body { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Test;
            yield return Body;
        }
    }
}
