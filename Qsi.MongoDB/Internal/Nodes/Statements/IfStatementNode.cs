using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class IfStatementNode : BaseNode, IStatementNode
{
    public IExpressionNode Test { get; set; }

    public IStatementNode Consequent { get; set; }

    public IStatementNode Alternate { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Test;
            yield return Consequent;
            yield return Alternate;
        }
    }
}
