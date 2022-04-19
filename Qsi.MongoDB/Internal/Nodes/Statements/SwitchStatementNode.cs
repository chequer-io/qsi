using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class SwitchStatementNode : BaseNode, IStatementNode
{
    public IExpressionNode Discriminant { get; set; }

    public SwitchCaseNode[] Cases { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Discriminant;

            foreach (var @case in Cases)
                yield return @case;
        }
    }
}
